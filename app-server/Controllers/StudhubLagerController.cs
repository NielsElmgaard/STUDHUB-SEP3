using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Studhub.AppServer.Services.Lager;
using StudHub.SharedDTO.Inventory;
using StudHub.SharedDTO.Lager;

namespace Studhub.AppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class StudhubLagerController : ControllerBase
{
    private readonly StudhubLagerService _service;

    public StudhubLagerController(StudhubLagerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult> GetLager([FromQuery] int studUserId,
        [FromQuery] string? search, [FromQuery] string? color, [FromQuery] string? itemType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("Invalid page or pageSize value.");
        }

        var inventoryList =
            await _service.GetAllBrickLinkInventoryAsync(studUserId, page,
                pageSize, search,color,itemType);

        return Ok(inventoryList);
    }

    [HttpGet("{id:int}")]
    public async Task<LagerDetaljerDTO?> GetDetaljer(int id)
    {
        return await _service.HentElementDetaljerAsync(id);
    }
}