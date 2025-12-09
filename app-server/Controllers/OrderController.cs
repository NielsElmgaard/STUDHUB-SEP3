using Microsoft.AspNetCore.Mvc;
using Studhub.AppServer.Services.Order;

namespace Studhub.AppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController:ControllerBase
{
    private readonly IOrderService _orderService;
    
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpGet("bricklink-orders")]
    public async Task<ActionResult> GetBricklinkOrders([FromQuery] int studUserId)
    {
        var orders = await _orderService.GetBricklinikOrderAsync(studUserId);
        return Ok(orders);
    }
    
}