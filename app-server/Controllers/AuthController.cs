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

    [HttpPut("bricklink-connect")]
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

    [HttpPut("brickowl-connect")]
    public async Task<ActionResult<BrickOwlCredentialsResponseDTO>>
        SetBrickOwlCredentials(
            [FromBody] BrickOwlCredentialsRequestDTO request)
    {
        await _authService.SetBrickOwlCredentialsAsync(
            request.StudUserId,
            request.BrickOwlApiKey);

        return Ok(new BrickOwlCredentialsResponseDTO
        {
            IsSucces = true
        });
    }

    [HttpDelete("bricklink-connect/{studUserId:long}")]
    public async Task<ActionResult<BrickLinkCredentialsResponseDTO>>
        ClearBrickLinkCredentials(long studUserId)
    {
        await _authService.ClearBrickLinkCredentialsAsync(studUserId, "",
            "", "", "");

        return NoContent();
    }

    [HttpDelete("brickowl-connect/{studUserId:long}")]
    public async Task<ActionResult<BrickOwlCredentialsResponseDTO>>
        ClearBrickOwlCredentials(long studUserId)
    {
        await _authService.ClearBrickOwlCredentialsAsync(studUserId, "");

        return NoContent();
    }
    
    [HttpGet("bricklink-status/{studUserId:long}")]
    public async Task<ActionResult> GetBrickLinkConnectionStatus(long studUserId)
    {
        bool isConnected = await _authService.IsBrickLinkConnectedAsync(studUserId);

        if (isConnected)
        {
            return Ok();
        }

        return NotFound();
    }
    
    [HttpGet("brickowl-status/{studUserId:long}")]
    public async Task<ActionResult> GetBrickOwlConnectionStatus(long studUserId)
    {
        bool isConnected = await _authService.IsBrickOwlConnectedAsync(studUserId);

        if (isConnected)
        {
            return Ok();
        }

        return NotFound();
    }
}