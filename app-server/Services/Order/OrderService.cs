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
    private readonly OrderClient _orderClinet;

    private static string brickLinkOrdersUrl =
        "https://api.bricklink.com/api/store/v1/orders";

    private static string brickOwlBaseOrderUrl =
        "https://api.brickowl.com/v1/order/";


    public OrderService(IApiAuthService apiAuthService, OrderClient orderClinet)
    {
        _apiAuthService = apiAuthService;
        _orderClinet = orderClinet;
    }

    public async Task<List<BrickLinkOrderDto>> GetBricklinkOrderAsync(int studUserId)
    {
        var desiredSatus = "Pending,Updated,Processing,Ready,Paid,Packed,Shipped,Received,Completed";
        var queryParams = new Dictionary<string, string> { { "status", desiredSatus } };
        var data = await _apiAuthService.GetBrickLinkResponse<BrickLinkOrderDto>(studUserId, brickLinkOrdersUrl,
            queryParams);
        return data;
    }

    public async Task<List<BrickOwlOrderItemDto>> GetBrickOwlOrderAsync(int studUserId)
    {
        // 1. Get last 1 hour order from BrickOWL
        var unixTimestamp = DateTimeOffset.UtcNow
            .AddHours(1)
            .ToUnixTimeSeconds(); // Only fetch orders happens last hour
        var quryParams = new Dictionary<string, string> { { "order_time", unixTimestamp.ToString() } };
        var data = await _apiAuthService.GetBrickLinkResponse<List<BrickOwlOrderListDto>>(studUserId,
            $"{brickOwlBaseOrderUrl}/list", quryParams);
        // 2. Get item Details from each order
        var allItems = new List<BrickOwlOrderItemDto>();
        foreach (var order in data)
        {
            var items = await _apiAuthService.GetBrickLinkResponse<List<BrickOwlOrderItemDto>>(studUserId,
                $"{brickOwlBaseOrderUrl}/items");
        }

        return allItems;
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
        List<BrickOwlOrderItemDto> brickowlOrderItems)
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

        return await _orderClinet.UpdateOrdersAsync(request);
    }
}