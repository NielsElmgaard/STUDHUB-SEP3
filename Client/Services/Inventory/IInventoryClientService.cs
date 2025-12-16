using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;

namespace Client.Services.Inventory;

public interface IInventoryClientService
{
    Task<List<BrickLinkInventoryDTO>> GetUserBrickLinkInventoryAsync(
        long studUserId); // TODO: Delete

    Task<List<BrickOwlLotDTO>> GetUserBrickOwlInventoryAsync(long studUserId); // TODO: Delete

    Task<List<string>> DiscoverBrickOwlInventoryKeysAsync(long studUserId); // TODO: Delete

    Task<List<string>> DiscoverBrickLinkInventoryKeysAsync(long studUserId); // TODO: Delete

    Task<PagedResultDTO<BrickLinkInventoryDTO>>
        GetUserBrickLinkInventoryFromDatabaseAsync(long studUserId, int page,
            int pageSize,string? searchText, string? selectedColor, string? selectedItemType);
}