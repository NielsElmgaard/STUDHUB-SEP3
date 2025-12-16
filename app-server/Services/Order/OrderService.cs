using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Studhub.AppServer.Services.Api_Auth;
using Studhub.Grpc.Data;
using StudHub.SharedDTO.Order;
using OrderClient = Studhub.Grpc.Data.OrderService.OrderServiceClient;

namespace Studhub.AppServer.Services.Order;

public class OrderService : IOrderService
{
    private readonly IApiAuthService _apiAuthService;
    private readonly OrderClient _orderClient;

    private static string brickLinkOrdersUrl =
        "https://api.bricklink.com/api/store/v1/orders";

    private static string brickOwlBaseOrderUrl =
        "https://api.brickowl.com/v1/order";

    
    public OrderService(IApiAuthService apiAuthService, OrderClient orderClient)
    {
        _apiAuthService = apiAuthService;
        _orderClient = orderClient;
    }

    public async Task<List<BrickLinkOrderDto>> GetBricklinkOrderAsync(int studUserId)
    {
        var desiredSatus = "Pending,Updated,Processing,Ready,Paid,Packed,Shipped,Received,Completed";
        var queryParams = new Dictionary<string, string> { { "status", desiredSatus } };
        var orders = await _apiAuthService.GetBrickLinkResponse<BrickLinkOrderDto>(studUserId, brickLinkOrdersUrl,
            queryParams);
        var res = await UpdateBricklinkOrderAsync(studUserId, orders);
        return orders;
    }

    public async Task<List<BrickOwlOrderListDto>> GetBrickOwlOrderAsync(int studUserId)
    {
        var quryParams = new Dictionary<string, string> { { "list_type", "store" } };
        var orders = await _apiAuthService.GetBrickOwlResponse<BrickOwlOrderListDto>(studUserId,
            $"{brickOwlBaseOrderUrl}/list", quryParams);
        Console.WriteLine(orders);
        var res = await UpdateBrickOwlOrderAsync(studUserId, orders);
        return orders;
    }


    public async Task<UpdateResponse> UpdateBricklinkOrderAsync(int studUserId, List<BrickLinkOrderDto> brickLinkOrders)
    {
        var request = new UpdateRequest
        {
            UserId = studUserId,
            Source = DataSource.Bricklink
        };
        foreach (var inv in brickLinkOrders)
        {
            var invJson = JsonSerializer.Serialize(inv);
            request.Inventories.Add(Struct.Parser.ParseJson(invJson));
        }

        return await _orderClinet.UpdateOrdersAsync(request);
    }

    public async Task<UpdateResponse> UpdateBrickOwlOrderAsync(int studUserId,
        List<BrickOwlOrderListDto> brickowlOrderItems)
    {
        var request = new UpdateRequest
        {
            UserId = studUserId,
            Source = DataSource.Brickowl
        };
        foreach (var inv in brickowlOrderItems)
        {
            var invJson = JsonSerializer.Serialize(inv);
            request.Inventories.Add(Struct.Parser.ParseJson(invJson));
        }
        return await _orderClient.UpdateOrdersAsync(request);
    }
}