using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;

namespace Studhub.AppServer.Services;

public interface IInventoryService
{
    Task<List<SetDTO>> GetUserSetsAsync(long studUserId);
    Task<List<BrickLinkInventoryDTO>> GetUserInventoryAsync(long studUserId);

}