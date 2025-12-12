using System.Text.Json;
using Studhub.Grpc.Data;
using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;
using StudHub.SharedDTO.Lager;

namespace Studhub.AppServer.Services.Lager;

public class StudhubLagerService : IStudhubLagerService
{
    private readonly StudhubLagerRepository _repository;
    private readonly InventoryService.InventoryServiceClient _inventoryClient;


    public StudhubLagerService(StudhubLagerRepository repository,
        InventoryService.InventoryServiceClient inventoryClient)
    {
        _repository = repository;
        _inventoryClient = inventoryClient;
    }

    public async Task<List<LagerItemDTO>> HentLagerOversigtAsync()
    {
        return await _repository.GetAllItemsAsync();
    }

    public async Task<LagerDetaljerDTO?> HentElementDetaljerAsync(int id)
    {
        return await _repository.GetItemDetailsAsync(id);
    }

    public async Task<PagedResultDTO<BrickLinkInventoryDTO>>
        GetAllBrickLinkInventoryAsync(int studUserId, int page, int pageSize)
    {
        var request = new BrickLinkInventoryRequest()
        {
            StudUserId = studUserId,
        };
        var jsonResponse =
            await _inventoryClient.GetBrickLinkInventoryAsync(request);

        if (!jsonResponse.IsSuccess)
        {
            throw new Exception(
                $"Failed to retrieve BrickLink Inventory: {jsonResponse.ErrorMessage}");
        }

        var inventory = new List<BrickLinkInventoryDTO>();


        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        foreach (var inventoryStruct in jsonResponse.Data)
        {
            string jsonString = inventoryStruct.ToString();

            try
            {
                var dto = JsonSerializer
                    .Deserialize<BrickLinkInventoryDTO>(jsonString,
                        options);

                if (dto != null)
                {
                    inventory.Add(dto);
                }
            }
            catch (JsonException e)
            {
                throw new JsonException(
                    $"Error deserializing BrickLinkInventoryResponse content for {studUserId}",
                    e);
            }
        }

        int skip = (page - 1) * pageSize;

        var pagedItems = inventory.Skip(skip).Take(pageSize).ToList();

        var pagedResult = new PagedResultDTO<BrickLinkInventoryDTO>
        {
            Items = pagedItems,
            TotalCount =
                inventory.Count,
            PageSize = pageSize,
            CurrentPage = page
        };

        return pagedResult;
    }
}