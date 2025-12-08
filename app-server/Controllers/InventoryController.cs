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
    private readonly InventoryService.InventoryServiceClient _inventoryClient;

    public InventoryController(IInventoryService inventoryService, InventoryService.InventoryServiceClient inventoryServiceClient)
    {
        _inventoryService = inventoryService;
        _inventoryClient = inventoryServiceClient;
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
        
        var request = new SetInventoryRequest()
        {
            UserId = studUserId,
            Source = InventorySource.Bricklink
        };
        foreach (var inv in inventories)
        {
            var invJson = JsonSerializer.Serialize(inv);
            request.Inventories.Add((Struct.Parser.ParseJson(invJson)));
        }
        var response =
            await _inventoryClient.SetInventoriesAsync(request);
        
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
        var request = new SetInventoryRequest()
        {
            UserId = studUserId,
            Source = InventorySource.Brickowl
        };
        foreach (var inv in inventories)
        {
            var invJson = JsonSerializer.Serialize(inv);
            request.Inventories.Add((Struct.Parser.ParseJson(invJson)));
            Console.WriteLine(request.Inventories);
        }
        
        var response =
            await _inventoryClient.SetInventoriesAsync(request);
        
        if (!response.IsSuccess)
        {
            return StatusCode(
                500,
                $"gRPC error: {response.ErrorMessage ?? "Unknown error"}");
        }
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
    
    [HttpPost("brickowl-lots")]
    public async Task<ActionResult> PostInventoryChange(
        [FromQuery] int studUserId)
    {
        var identifiers =
            await _inventoryService.PostInventoryChangesFromBLOrders(studUserId);
        return Ok(identifiers);
    }
}