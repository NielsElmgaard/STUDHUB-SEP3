using Microsoft.AspNetCore.Mvc;
using Studhub.AppServer.Services;
using Studhub.AppServer.Services.Order;
using Studhub.Grpc.Data;
using InventoryClient = Studhub.Grpc.Data.InventoryService.InventoryServiceClient;

namespace Studhub.AppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class SynchronizationController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IInventoryService _inventoryService;

    public SynchronizationController(IOrderService orderService, IInventoryService inventoryService,
        InventoryClient inventoryClient)
    {
        _orderService = orderService;
        _inventoryService = inventoryService;
    }

    [HttpPost("")]
    public async Task<ActionResult> Synchronize([FromQuery] int studUserId)
    {
        try
        {
            // 7 & 8 Get and update BrickLink Inventory
            var blInventories = await _inventoryService.GetUserBrickLinkInventoryAsync(studUserId);
            // 8. Update source of truth (BrickLink) Inventory
            var invResponse =
                await _inventoryService.UpdateInventoryAsync(studUserId, blInventories, DataSource.Bricklink);
            // 9. Update BrickOwl Inventory
            var boInventories = await _inventoryService.GetUserBrickOwlInventoryAsync(studUserId);
            await _inventoryService.UpdateInventoryAsync(studUserId, boInventories, DataSource.Brickowl);
            // 10. Update BrickOwl Inventory based on BrickLink
            var result = await _inventoryService.UpdateBrickOwlDiffInventoryAsync(studUserId);

            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = "An unexpected error occurred", details = e.Message });
        }
    }
}