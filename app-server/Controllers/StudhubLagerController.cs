using Microsoft.AspNetCore.Mvc;
using Studhub.AppServer.Services.Lager;
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
    public Task<List<LagerItemDTO>> GetLager()
    {
        return _service.HentLagerOversigtAsync();
    }
    
    [HttpGet("{id}")]
    public Task<LagerDetaljerDTO?> GetDetaljer(int id)
    {
        return _service.HentElementDetaljerAsync(id);
    }
    
    [HttpGet("search")]
    public Task<List<LagerItemDTO>> Search(
        [FromQuery(Name = "q")] string? searchText,
        [FromQuery] string? color,
        [FromQuery] string? partId,
        [FromQuery] string? platform)
    {
        return _service.SøgOgFiltrerAsync(
            searchText,
            color,
            partId,
            platform);
    }
}