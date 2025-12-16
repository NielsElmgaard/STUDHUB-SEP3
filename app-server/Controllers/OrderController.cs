using Microsoft.AspNetCore.Mvc;
using Studhub.AppServer.Services.Order;

namespace Studhub.AppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("bricklink")]
    public async Task<ActionResult> GetBrickLinkOrders([FromQuery] int studUserId)
    {
        var orders = await _orderService.GetBricklinkOrderAsync(studUserId);
        return Ok(orders);
    }

    [HttpGet("brickowl")]
    public async Task<ActionResult> GetBrickOwlOrders([FromQuery] int studUserId)
    {
        var orders = await _orderService.GetBrickOwlOrderAsync(studUserId);
        return Ok(orders);
    }
}