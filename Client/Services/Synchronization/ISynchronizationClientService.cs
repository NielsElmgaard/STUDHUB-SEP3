using StudHub.SharedDTO.Inventory;

namespace Client.Services.Synchronization;

public interface ISynchronizationClientService
{
    Task<SyncInventoryResponse> SynchronizeInventoryAsync(long studUserId);
    Task FetchBrickLinkOrdersAsync(long studUserId);
    Task FetchBrickOwlOrdersAsync(long studUserId);
}