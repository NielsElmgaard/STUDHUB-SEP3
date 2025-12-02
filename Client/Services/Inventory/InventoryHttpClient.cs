using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;

namespace Client.Services;

public class InventoryHttpClient : IInventoryClientService
{
    private readonly HttpClient _httpClient;

    public InventoryHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SetDTO>> GetUserSetsAsync(long studUserId)
    {
        var response =
            await _httpClient.GetAsync(
                $"Inventory/sets?studUserId={studUserId}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching sets: {error}");
        }

        var sets = await response.Content.ReadFromJsonAsync<List<SetDTO>>();
        return sets ?? new List<SetDTO>();
    }

    public async Task<List<BrickLinkInventoryDTO>>
        GetUserBrickLinkInventoryAsync(long studUserId)
    {
        var response =
            await _httpClient.GetAsync(
                $"Inventory/bricklink?studUserId={studUserId}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching inventories: {error}");
        }

        var inventories = await response.Content
            .ReadFromJsonAsync<List<BrickLinkInventoryDTO>>();

        return inventories ?? new List<BrickLinkInventoryDTO>();
    }

    public async Task<List<BrickOwlLotDTO>> GetUserBrickOwlInventoryAsync(
        long studUserId)
    {
        var response =
            await _httpClient.GetAsync(
                $"Inventory/brickowl?studUserId={studUserId}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching inventories: {error}");
        }

        var inventories = await response.Content
            .ReadFromJsonAsync<List<BrickOwlLotDTO>>();

        return inventories ?? new List<BrickOwlLotDTO>();
    }

    public async Task<List<string>> DiscoverBrickOwlInventoryKeysAsync(
        long studUserId)
    {
        var response =
            await _httpClient.GetAsync(
                $"Inventory/brickowl-identifiers?studUserId={studUserId}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching inventories: {error}");
        }

        var identifiers = await response.Content
            .ReadFromJsonAsync<List<string>>();

        return identifiers ?? new List<string>();
    }

    public async Task<List<string>> DiscoverBrickLinkInventoryKeysAsync(
        long studUserId)
    {
        var response =
            await _httpClient.GetAsync(
                $"Inventory/bricklink-identifiers?studUserId={studUserId}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching BrickLink keys: {error}");
        }

        var identifiers = await response.Content
            .ReadFromJsonAsync<List<string>>();

        return identifiers ?? new List<string>();
    }
}