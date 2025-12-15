using System.Net.Http.Json;
using StudHub.SharedDTO.Inventory;

namespace Client.Services.Synchronization;

public class SynchronizationHttpClient : ISynchronizationClientService
{
    private readonly HttpClient _httpClient;

    public SynchronizationHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SyncInventoryResponse> SynchronizeInventoryAsync(long studUserId)
    {
        var response = await _httpClient.PostAsync(
            $"synchronization?studUserId={studUserId}",
            content: null);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error synchronizing inventory: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<SyncInventoryResponse>();
        return result ?? new SyncInventoryResponse();
    }

    public async Task FetchBrickLinkOrdersAsync(long studUserId)
    {
        var response = await _httpClient.GetAsync($"order/bricklink?studUserId={studUserId}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching BrickLink orders: {error}");
        }
    }

    public async Task FetchBrickOwlOrdersAsync(long studUserId)
    {
        var response = await _httpClient.GetAsync($"order/brickowl?studUserId={studUserId}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching BrickOwl orders: {error}");
        }
    }
}