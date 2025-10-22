using Microsoft.AspNetCore.Mvc;
using Studhub.AppServer.Services;

namespace Studhub.AppServer.Controllers;

[ApiController]
[Route("[controller]")]

public class SetsController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public SetsController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<ActionResult> GetSets([FromQuery] string email)
    {
        var sets = await _inventoryService.GetUserSetsAsync(email);
        return Ok(sets);
    }
}