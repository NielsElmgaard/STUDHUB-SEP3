using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;

namespace Studhub.AppServer.Services;

public interface IInventoryService
{
    Task<List<SetDTO>> GetUserSetsAsync(long studUserId);
    Task<List<BrickLinkInventoryDTO>> GetUserBrickLinkInventoryAsync(long studUserId);

    Task<List<BrickOwlLotDTO>> GetUserBrickOwlInventoryAsync(long studUserId);
    Task<List<string>> DiscoverBrickOwlInventoryKeysAsync(long studUserId);
    Task<List<string>> DiscoverBrickLinkInventoryKeysAsync(long studUserId);

}