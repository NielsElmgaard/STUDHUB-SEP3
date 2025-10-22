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
    public async Task<ActionResult> GetSets([FromQuery] string email)
    {
        var sets = await _inventoryService.GetUserSetsAsync(email);
        return Ok(sets);
    }
}