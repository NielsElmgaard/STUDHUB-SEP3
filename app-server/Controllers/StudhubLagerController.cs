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
    public async Task<List<LagerItemDTO>> GetLager()
    {
        return await _service.HentLagerOversigtAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<LagerDetaljerDTO?> GetDetaljer(int id)
    {
        return await _service.HentElementDetaljerAsync(id);
    }
}