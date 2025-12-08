using System.Text.Json;
using Studhub.AppServer.Services.Api_Auth;
using Studhub.AppServer.Services.Auth_Login;
using StudHub.SharedDTO.Order;

namespace Studhub.AppServer.Services.Order;

public class OrderService : IOrderService
{
    private readonly IApiAuthService _apiAuthService;

    private static string brickLinkOrdersUrl =
        "https://api.bricklink.com/api/store/v1/orders";

    private static string brickOwlOrderUrl =
        "https://api.brickowl.com/v1/order/list";

    
    public OrderService(IApiAuthService apiAuthService)
    {
        _apiAuthService = apiAuthService;
    }

    public async Task<List<BrickLinkOrderDTO>> GetBricklinikOrderAsync(int studUserId)
    {
        try
        {
            var desiredSatus = "Pending,Updated,Processing,Ready,Paid,Packed,Shipped,Received,Completed";
            var quryParams = new Dictionary<string, string> { { "status", desiredSatus } };
            var data = await _apiAuthService.GetBrickLinkResponse<BrickLinkOrderDTO>(studUserId, brickLinkOrdersUrl, quryParams);
            return data;
        }
        catch (JsonException e)
        {
            throw new JsonException(
                $"Error deserializing BrickLink response content for {studUserId}",
                e);
        }
        catch (Exception e)
        {
            throw new Exception(
                $"An error occurred during fetching order from BrickLink for {studUserId}",
                e);
        }
    }
}