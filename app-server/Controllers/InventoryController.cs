using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Studhub.AppServer.Services;
using Studhub.Grpc.Data;

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
    
    [HttpGet]
    public async Task<ActionResult> GetStudHubStorage([FromQuery] int studUserId,
        [FromQuery] string? search, [FromQuery] string? color, [FromQuery] string? itemType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("Invalid page or pageSize value.");
        }

        var inventoryList =
            await _inventoryService.GetAllBrickLinkInventoryFromDbAsync(studUserId, page,
                pageSize, search,color,itemType);

        return Ok(inventoryList);
    }
    
    [HttpGet("bricklink")]
    public async Task<ActionResult> GetAllBrickLinkInventoriesForStud(
        [FromQuery] int studUserId)
    {
        var inventories =
            await _inventoryService.GetUserBrickLinkInventoryAsync(studUserId);
        
        var response =
            await _inventoryService.UpdateInventoryAsync(studUserId, inventories, DataSource.Bricklink);
        
        if (!response.IsSuccess)
        {
            return StatusCode(
                500,
                $"gRPC error: {response.ErrorMessage ?? "Unknown error"}");
        }
        return Ok(inventories);
    }
    
    [HttpGet("brickowl")]
    public async Task<ActionResult> GetAllBrickOwlInventoriesForStud(
        [FromQuery] int studUserId)
    {
        var inventories =
            await _inventoryService.GetUserBrickOwlInventoryAsync(studUserId);
        return Ok(inventories);
    }

    // TODO: DELETE
    [HttpGet("brickowl-identifiers")]
    public async Task<ActionResult> DiscoverBrickOwlInventoryKeysAsync(
        [FromQuery] int studUserId)
    {
        var identifiers =
            await _inventoryService.DiscoverBrickOwlInventoryKeysAsync(studUserId);
        return Ok(identifiers);
    }
    
    // TODO: DELETE
    [HttpGet("bricklink-identifiers")]
    public async Task<ActionResult> DiscoverBrickLinkInventoryKeysAsync(
        [FromQuery] int studUserId)
    {
        var identifiers =
            await _inventoryService.DiscoverBrickLinkInventoryKeysAsync(studUserId);
        return Ok(identifiers);
    }
    
    
}