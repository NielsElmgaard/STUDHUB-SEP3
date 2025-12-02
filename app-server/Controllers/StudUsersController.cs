using Microsoft.AspNetCore.Mvc;
using Studhub.AppServer.Services.StudUser;
using StudHub.SharedDTO.Users;

namespace Studhub.AppServer.Controllers;

[ApiController]
[Route("[controller]")]
public class StudUsersController : ControllerBase
{
    private readonly IStudUserService _studUserService;

    public StudUsersController(IStudUserService studUserService)
    {
        _studUserService = studUserService;
    }

    [HttpPost]
    public async Task<ActionResult<StudUserDTO>> CreateStudUser(
        [FromBody] CreateStudUserRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new CreateStudUserResponseDTO
            {
                IsSuccess = false,
                ErrorMessage = "Email is required and cannot be empty"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest(new CreateStudUserResponseDTO
            {
                IsSuccess = false,
                ErrorMessage = "Username is required and cannot be empty"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new CreateStudUserResponseDTO
            {
                IsSuccess = false,
                ErrorMessage = "Password is required and cannot be empty"
            });
        }


        var grpcResponse = await _studUserService.CreateStudUser(request);

        if (!grpcResponse.IsSuccess)
        {
            if (grpcResponse.ErrorMessage.Contains("email already exists"))
            {
                return Conflict(grpcResponse);
            }

            return BadRequest(grpcResponse);
        }

        return Created($"/StudUsers/{grpcResponse.Id}", request);
    }
}