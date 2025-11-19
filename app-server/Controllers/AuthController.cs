using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Studhub.AppServer.Services;
using Studhub.AppServer.Services.Auth_Login;
using StudHub.SharedDTO;
using StudHub.SharedDTO.StoreCredentials;
using StudHub.SharedDTO.Users;

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
        var studUser =
            await _authService.ValidateUserAsync(request.Email,
                request.Password);

        if (studUser == null)
        {
            return BadRequest(new LoginResponseDTO
                { ErrorMessage = "Invalid email or password" });
        }

        return Ok(new LoginResponseDTO
        {
            Username = studUser.Username,
            StudUser = studUser
        });
    }

    [HttpPut("bricklinksync")]
    public async Task<ActionResult<BrickLinkCredentialsResponseDTO>>
        SetBrickLinkCredentials(
            [FromBody] BrickLinkCredentialsRequestDTO request)
    {
       await _authService.SetBrickLinkCredentialsAsync(
            request.StudUserId,
            request.ConsumerKey, request.ConsumerSecret, request.TokenValue,
            request.TokenSecret);

        return Ok(new BrickLinkCredentialsResponseDTO
        {
            IsSucces = true
        });
    }

    [HttpPut("brickowlsync")]
    public async Task<ActionResult<BrickOwlCredentialsResponseDTO>>
        SetBrickOwlCredentials(
            [FromBody] BrickOwlCredentialsRequestDTO request)
    {
        var credentials = await _authService.SetBrickOwlCredentialsAsync(
            request.StudUserId,
            request.BrickOwlApiKey);

        return Ok(new BrickLinkCredentialsResponseDTO
        {
            IsSucces = true
        });
    }
}