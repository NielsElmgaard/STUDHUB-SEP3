using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;

namespace Client.Services;

public interface IInventoryClientService
{
    Task<List<SetDTO>> GetUserSetsAsync(long studUserId);
    Task<List<BrickLinkInventoryDTO>> GetUserBrickLinkInventoryAsync(long studUserId);
    Task<List<BrickOwlLotDTO>> GetUserBrickOwlInventoryAsync(long studUserId);

    Task<List<string>> DiscoverBrickOwlInventoryKeysAsync(long studUserId);
    
    Task<List<string>> DiscoverBrickLinkInventoryKeysAsync(long studUserId);

}