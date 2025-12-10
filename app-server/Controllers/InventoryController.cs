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

    [HttpGet("sets")]
    public async Task<ActionResult> GetSets([FromQuery] int studUserId)
    {
        var sets = await _inventoryService.GetUserSetsAsync(studUserId);
        return Ok(sets);
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

    [HttpGet("brickowl-identifiers")]
    public async Task<ActionResult> DiscoverBrickOwlInventoryKeysAsync(
        [FromQuery] int studUserId)
    {
        var identifiers =
            await _inventoryService.DiscoverBrickOwlInventoryKeysAsync(studUserId);
        return Ok(identifiers);
    }
    
    [HttpGet("bricklink-identifiers")]
    public async Task<ActionResult> DiscoverBrickLinkInventoryKeysAsync(
        [FromQuery] int studUserId)
    {
        var identifiers =
            await _inventoryService.DiscoverBrickLinkInventoryKeysAsync(studUserId);
        return Ok(identifiers);
    }
}