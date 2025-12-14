using System.Globalization;
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
        GetAllBrickLinkInventoryAsync(int studUserId, int page, int pageSize,
            string? search, string? color, string? itemType)
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

        IEnumerable<BrickLinkInventoryDTO> filteredInventory = inventory;

        if (!string.IsNullOrWhiteSpace(search))
        {
            string lowerSearch = search.ToLowerInvariant();

            filteredInventory = inventory.Where(item =>
                item.Item?.Name?.ToLowerInvariant().Contains(lowerSearch) ==
                true ||
                item.Item?.No?.ToLowerInvariant().Contains(lowerSearch) ==
                true ||
                item.NewOrUsed?.ToLowerInvariant().Contains(lowerSearch) ==
                true ||
                item.Remarks?.ToLowerInvariant().Contains(lowerSearch) ==
                true ||
                item.Description?.ToLowerInvariant().Contains(lowerSearch) ==
                true ||
                item.InventoryId.ToString().Contains(lowerSearch) ||
                item.UnitPrice?.Contains(lowerSearch) == true ||
                item.DateCreated.ToString().ToLowerInvariant()
                    .Contains(lowerSearch));
        }

        // For dropdown menu of colors
        if (!string.IsNullOrWhiteSpace(color))
        {
            string lowerColor = color.ToLowerInvariant();

            filteredInventory = filteredInventory.Where(item =>
                item.ColorName?.ToLowerInvariant().Equals(lowerColor) == true);
        }
        
        // For dropdown menu of type
        if (!string.IsNullOrWhiteSpace(itemType))
        {
            string lowerItemType = itemType.ToLowerInvariant();

            filteredInventory = filteredInventory.Where(item =>
                item.Item.Type?.ToLowerInvariant().Equals(lowerItemType) == true);
        }
        

        int totalCount = filteredInventory.Count();
        int skip = (page - 1) * pageSize;

        var pagedItems = filteredInventory.Skip(skip).Take(pageSize).ToList();

        var pagedResult = new PagedResultDTO<BrickLinkInventoryDTO>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = page
        };

        return pagedResult;
    }
}