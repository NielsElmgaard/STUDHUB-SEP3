using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;

namespace Client.Services;

public interface IInventoryClientService
{
    Task<List<SetDTO>> GetUserSetsAsync(long studUserId);
    Task<List<BrickLinkInventoryDTO>> GetUserInventoryAsync(long studUserId);
}