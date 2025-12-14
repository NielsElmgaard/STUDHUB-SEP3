using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;

namespace Client.Services.Inventory;

public interface IInventoryClientService
{
    Task<List<SetDTO>> GetUserSetsAsync(long studUserId);

    Task<List<BrickLinkInventoryDTO>> GetUserBrickLinkInventoryAsync(
        long studUserId);

    Task<List<BrickOwlLotDTO>> GetUserBrickOwlInventoryAsync(long studUserId);

    Task<List<string>> DiscoverBrickOwlInventoryKeysAsync(long studUserId);

    Task<List<string>> DiscoverBrickLinkInventoryKeysAsync(long studUserId);

    Task<PagedResultDTO<BrickLinkInventoryDTO>>
        GetUserBrickLinkInventoryFromDatabaseAsync(long studUserId, int page,
            int pageSize,string? searchText, string? selectedColor, string? selectedItemType);
}