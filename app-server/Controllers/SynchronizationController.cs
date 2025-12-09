using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Studhub.AppServer.Services;
using Studhub.AppServer.Services.Order;

namespace Studhub.AppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class SynchronizationController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IInventoryService _inventoryService;
    
    public SynchronizationController(IOrderService orderService, IInventoryService inventoryService)
    {
        _orderService = orderService;
        _inventoryService = inventoryService;
    }
    
    [HttpPost("")]
    public async Task<ActionResult> synchronize([FromQuery] int studUserId)
    {
        // 1. Get BrickLink Orders
        var blOrders = await _orderService.GetBricklinikOrderAsync(studUserId);
        // 2. Update BrickLink Orders to data server
        var blResponse =
            await _orderService.UpdateBricklinkOrderAsync(studUserId, blOrders);
        // 3. Post Inventory Change to BrickOwl based on BrickLink Orders
        // 4. Get BrickOwl Orders
        // ???? what is brickowl returning????
        // 5. Update BrickOwl Orders to data server
        // 6. Post Inventory Change to BrickLink based on BrickLink Orders
        // 7. Get BrickLink Inventory
        // var blInventories = await _inventoryService.GetUserBrickLinkInventoryAsync(studUserId);
        // 8. Update source of truth (BrickLink) Inventory
        // var invResponse = await _inventoryService.UpdateBrickLinkInventoryAsync(studUserId, blInventories);
        // 9. Get BrickOwl Inventory
        var boInventories = await _inventoryService.GetUserBrickOwlInventoryAsync(studUserId);
        // 10. Update BrickOwl Inventory
        
        
        // TODO: adjust response
        return Ok(1);
    }
}