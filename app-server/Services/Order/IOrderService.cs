using Studhub.Grpc.Data;
using StudHub.SharedDTO.Order;

namespace Studhub.AppServer.Services.Order;

public interface IOrderService
{
    Task<List<BrickLinkOrderDto>> GetBricklinkOrderAsync(int studUserId);
    Task<List<BrickOwlOrderItemDto>> GetBrickOwlOrderAsync(int studUserId);
    Task<UpdateResponse> UpdateBricklinkOrderAsync(int studUserId, List<BrickLinkOrderDto> brickLinkOrders);
    Task<UpdateResponse> UpdateBrickOwlOrderAsync(int studUserId, List<BrickOwlOrderItemDto> brickowlOrder);
}