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
            throw new ArgumentException(
                $"Email is required and cannot be empty");
        }
        
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException(
                $"Username is required and cannot be empty");
        }
        
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException(
                $"Password is required and cannot be empty");
        }
        
        
        var grpcResponse = await _studUserService.CreateStudUser(request);

        var response = new CreateStudUserResponseDTO
        {
            IsSuccess = grpcResponse.IsSuccess,
            ErrorMessage = grpcResponse.ErrorMessage,
            Id = grpcResponse.Id
        };
        
        return Created($"/StudUsers/{response.Id}", request);
    }
}