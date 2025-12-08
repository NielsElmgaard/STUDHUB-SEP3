using StudHub.SharedDTO.Order;

namespace Studhub.AppServer.Services.Order;

public interface IOrderService
{
    Task<List<BrickLinkOrderDTO>> GetBricklinikOrderAsync(int studUserId);
}