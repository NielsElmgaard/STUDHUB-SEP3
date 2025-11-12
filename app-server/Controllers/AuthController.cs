using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Studhub.AppServer.Services;
using Studhub.AppServer.Services.Auth_Login;
using StudHub.SharedDTO;

namespace Studhub.AppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login(
        [FromBody] LoginRequestDTO request)
    {
        var username = await _authService.ValidateUserAsync(request.Email,
            request.Password);
        if (username == null)
            return BadRequest(new LoginResponseDTO
                { ErrorMessage = "Invalid email or password" });
        return Ok(new LoginResponseDTO { Username = username });
    }
}