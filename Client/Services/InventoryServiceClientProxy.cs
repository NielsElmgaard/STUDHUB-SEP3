using Studhub.AppServer.Services;
using StudHub.SharedDTO;

namespace Client.Services;

public class InventoryServiceClientProxy : IInventoryService
{
    private readonly HttpClient _httpClient;

    public InventoryServiceClientProxy(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SetDTO>> GetUserSetsAsync(string email)
    {
        var response =
            await _httpClient.GetAsync($"Inventory/sets?email={email}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching sets: {error}");
        }

        var sets = await response.Content.ReadFromJsonAsync<List<SetDTO>>();
        return sets ?? new List<SetDTO>();
    }
}