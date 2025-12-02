using Microsoft.AspNetCore.Mvc;
using Studhub.AppServer.Services;

namespace Studhub.AppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("sets")]
    public async Task<ActionResult> GetSets([FromQuery] long studUserId)
    {
        var sets = await _inventoryService.GetUserSetsAsync(studUserId);
        return Ok(sets);
    }

    [HttpGet("bricklink")]
    public async Task<ActionResult> GetAllBrickLinkInventoriesForStud(
        [FromQuery] long studUserId)
    {
        var inventories =
            await _inventoryService.GetUserBrickLinkInventoryAsync(studUserId);
        return Ok(inventories);
    }
    
    [HttpGet("brickowl")]
    public async Task<ActionResult> GetAllBrickOwlInventoriesForStud(
        [FromQuery] long studUserId)
    {
        var inventories =
            await _inventoryService.GetUserBrickOwlInventoryAsync(studUserId);
        return Ok(inventories);
    }

    [HttpGet("brickowl-identifiers")]
    public async Task<ActionResult> DiscoverBrickOwlInventoryKeysAsync(
        [FromQuery] long studUserId)
    {
        var identifiers =
            await _inventoryService.DiscoverBrickOwlInventoryKeysAsync(studUserId);
        return Ok(identifiers);
    }
    
    [HttpGet("bricklink-identifiers")]
    public async Task<ActionResult> DiscoverBrickLinkInventoryKeysAsync(
        [FromQuery] long studUserId)
    {
        var identifiers =
            await _inventoryService.DiscoverBrickLinkInventoryKeysAsync(studUserId);
        return Ok(identifiers);
    }
}