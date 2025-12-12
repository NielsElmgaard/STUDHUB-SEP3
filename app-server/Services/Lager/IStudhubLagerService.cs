using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;

namespace Studhub.AppServer.Services.Lager;

public interface IStudhubLagerService
{
    Task<PagedResultDTO<BrickLinkInventoryDTO>> GetAllBrickLinkInventoryAsync(
        int studUserId, int page, int pageSize);
}