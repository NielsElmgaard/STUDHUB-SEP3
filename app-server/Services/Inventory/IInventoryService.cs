using Studhub.Grpc.Data;
using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;

namespace Studhub.AppServer.Services;

public interface IInventoryService
{
    Task<List<SetDTO>> GetUserSetsAsync(int studUserId);
    Task<List<BrickLinkInventoryDTO>> GetUserBrickLinkInventoryAsync(int studUserId);
    Task<List<BrickOwlLotDTO>> GetUserBrickOwlInventoryAsync(int studUserId);
    Task<List<string>> DiscoverBrickOwlInventoryKeysAsync(int studUserId);
    Task<List<string>> DiscoverBrickLinkInventoryKeysAsync(int studUserId);

    Task<UpdateResponse> UpdateInventoryAsync<T>(int studUserId, List<T> inventories, DataSource source);
    Task<Dictionary<string, List<string>>> UpdateBrickOwlDiffInventoryAsync(int studUserId);
}