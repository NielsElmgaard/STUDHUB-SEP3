using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Studhub.AppServer.Services.Api_Auth;
using Studhub.AppServer.Services.Auth_Login;
using Studhub.Grpc.Data;
using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;
using InventoryClient =
    Studhub.Grpc.Data.InventoryService.InventoryServiceClient;

namespace Studhub.AppServer.Services.Inventory;

public class InventoryService : IInventoryService
{
    private static readonly string brickLinkInventoriesUrl =
        "https://api.bricklink.com/api/store/v1/inventories";

    private static readonly string brickOwlInventoriesBaseUrl =
        "https://api.brickowl.com/v1/inventory";

    private readonly IApiAuthService _apiAuthService;
    private readonly IAuthService _authService;
    private readonly HttpClient _httpClient;
    private readonly InventoryClient _inventoryClient;

    public InventoryService(IAuthService authService,
        HttpClient httpClient, IApiAuthService apiAuthService,
        InventoryClient inventoryClient)
    {
        _authService = authService;
        _httpClient = httpClient;
        _apiAuthService = apiAuthService;
        _inventoryClient = inventoryClient;
    }


    public async Task<List<BrickLinkInventoryDTO>>
        GetUserBrickLinkInventoryAsync(
            int studUserId)
    {
        return await
            _apiAuthService.GetBrickLinkResponse<BrickLinkInventoryDTO>(
                studUserId, brickLinkInventoriesUrl);
    }

    public async Task<List<BrickOwlLotDTO>> GetUserBrickOwlInventoryAsync(
        int studUserId)
    {
        return await _apiAuthService.GetBrickOwlResponse<BrickOwlLotDTO>(
            studUserId,
            $"{brickOwlInventoriesBaseUrl}/list");
    }

    public async Task<List<string>> DiscoverBrickOwlInventoryKeysAsync(
        int studUserId)
    {
        var credentials =
            await _authService.GetBrickOwlCredentialsAsync(studUserId);
        if (credentials == null)
            throw new Exception($"No Brick Owl credentials for {studUserId}");

        var fullUrl =
            $"{brickOwlInventoriesBaseUrl}/list?key={credentials.BrickOwlApiKey}";
        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var uniqueKeys = new HashSet<string>();

        try
        {
            var lotElements =
                JsonSerializer.Deserialize<JsonElement[]>(jsonResponse);

            if (lotElements == null || lotElements.Length == 0)
                return new List<string>();

            var firstLot = lotElements[0];

            if (firstLot.ValueKind != JsonValueKind.Object)
                throw new JsonException(
                    "First element in inventory array is not a JSON object.");

            foreach (var property in firstLot.EnumerateObject())
                uniqueKeys.Add(property.Name);

            return uniqueKeys.ToList();
        }
        catch (JsonException e)
        {
            throw new JsonException(
                $"Error parsing Brick Owl response content for key discovery for {studUserId}. Check if JSON is valid.",
                e);
        }
        catch (Exception e)
        {
            throw new Exception(
                $"An error occurred during key discovery for {studUserId}",
                e);
        }
    }

    public async Task<List<string>> DiscoverBrickLinkInventoryKeysAsync(
        int studUserId)
    {
        var credentials =
            await _authService.GetBrickLinkCredentialsAsync(studUserId);
        if (credentials == null)
            throw new Exception($"No BrickLink credentials for {studUserId}");

        var jsonResponse = await OAuthHelper.ExecuteSignedApiCallAsync(
            _httpClient, brickLinkInventoriesUrl, HttpMethod.Get,
            credentials.ConsumerKey,
            credentials.ConsumerSecret, credentials.TokenValue,
            credentials.TokenSecret);

        var uniqueKeys = new HashSet<string>();

        try
        {
            using (var document = JsonDocument.Parse(jsonResponse))
            {
                var root = document.RootElement;

                if (!root.TryGetProperty("data", out var dataElement) ||
                    dataElement.ValueKind != JsonValueKind.Array)
                    throw new JsonException(
                        "BrickLink response 'data' property is missing or not an array.");

                if (dataElement.GetArrayLength() == 0)
                    return new List<string>();

                var firstLot = dataElement[0];

                if (firstLot.ValueKind != JsonValueKind.Object)
                    throw new JsonException(
                        "First element in data array is not a JSON object.");

                foreach (var property in firstLot.EnumerateObject())
                    uniqueKeys.Add(property.Name);
            }

            return uniqueKeys.ToList();
        }
        catch (JsonException e)
        {
            throw new JsonException(
                $"Error parsing BrickLink response content for key discovery for {studUserId}. Check JSON structure.",
                e);
        }
        catch (Exception e)
        {
            throw new Exception(
                $"An error occurred during key discovery for {studUserId}",
                e);
        }
    }

    public async Task<UpdateResponse> UpdateInventoryAsync<T>(int studUserId,
        List<T> inventories, DataSource source)
    {
        var request = new UpdateRequest
        {
            UserId = studUserId,
            Source = source
        };
        foreach (var inv in inventories)
        {
            var invJson = JsonSerializer.Serialize(inv);
            request.Inventories.Add(Struct.Parser.ParseJson(invJson));
        }

        return
            await _inventoryClient.SetInventoriesAsync(request);
    }

    /*
     * Update BrickOwl Inventory based on difference from BrickLink Inventory
     */
    public async Task<Dictionary<string, List<string>>> UpdateBrickOwlDiffInventoryAsync(int studUserId)
    {
        var request = new UserId { Id = studUserId };
        var diffInventories = await _inventoryClient.GetDiffInventoryForBrickOwlAsync(request);

        var updateResult = new Dictionary<string, List<string>>
        {
            { "success", new List<string>() },
            { "failed", new List<string>() }
        };

        // https://chatgpt.com:
        // Semaphore is used to proceed multiple calls simultaneously
        var semaphore = new SemaphoreSlim(5); 

        var tasks = diffInventories.Diffs.Select(async diff =>
        {
            await semaphore.WaitAsync();
            try
            {
                var formData = new Dictionary<string, string>
                {
                    { "type", diff.Type },
                    { "quantity", diff.Quantity },
                    { "price", diff.Price },
                    { "boid", diff.Boid },
                    { "condition", "new" }
                };

                if (diff.Action == "CREATE")
                {
                    var res = await _apiAuthService.PostBrickOwlResponse<BrickOwlCreateLotDTO>(studUserId, $"{brickOwlInventoriesBaseUrl}/create", formData);
                    lock(updateResult) { updateResult["success"].Add(res.LotId); }
                }
                else if (diff.Action == "UPDATE")
                {
                    formData.Add("lot_id", diff.LotId);
                    var res = await _apiAuthService.PostBrickOwlResponse<BrickOwlUpdateLotDto>(studUserId, $"{brickOwlInventoriesBaseUrl}/update", formData);
                    lock(updateResult) { updateResult["success"].Add(diff.LotId); }
                }
            }
            catch (Exception e)
            {
                lock(updateResult) { updateResult["failed"].Add($"BOID {diff.Boid}: {e.Message}"); }
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        return updateResult;
    }

    public async Task<PagedResultDTO<BrickLinkInventoryDTO>>
        GetAllBrickLinkInventoryFromDbAsync(int studUserId, int page,
            int pageSize,
            string? search, string? color, string? itemType)
    {
        var request = new UserId()
        {
            Id = studUserId,
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
                item.Item.Type?.ToLowerInvariant().Equals(lowerItemType) ==
                true);
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

public async Task<UpdateResponse> UpdateBrickOwlInventoryAsync(int studUserId,
    List<BrickLinkInventoryDTO> inventories)
{
    var request = new UpdateRequest
    {
        UserId = studUserId,
        Source = DataSource.Brickowl
    };
    foreach (var inv in inventories)
    {
        var invJson = JsonSerializer.Serialize(inv);
        request.Inventories.Add(Struct.Parser.ParseJson(invJson));
    }

    return
        await _inventoryClient.SetInventoriesAsync(request);
}

}