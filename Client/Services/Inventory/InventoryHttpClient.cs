using StudHub.SharedDTO;

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
            await _httpClient.GetAsync($"Inventory/sets?studUserId={studUserId}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching sets: {error}");
        }

        var sets = await response.Content.ReadFromJsonAsync<List<SetDTO>>();
        return sets ?? new List<SetDTO>();
    }
}