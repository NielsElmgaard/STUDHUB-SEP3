using Studhub.Grpc.Data;
using StudHub.SharedDTO.Users;

namespace Studhub.AppServer.Services.StudUser;

public class StudUserService : IStudUserService
{
    private readonly StudService.StudServiceClient _grpcClient;

    public StudUserService(StudService.StudServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<CreateStudUserResponseDTO> CreateStudUser(CreateStudUserRequestDTO userRequest)
    {
        var grpcResponse = await _grpcClient.CreateStudAsync(
            new CreateStudRequest
            {
                Email = userRequest.Email,
                Password = userRequest.Password,
                Username = userRequest.Username
            });

        return new CreateStudUserResponseDTO
        {
            IsSuccess = grpcResponse.IsSuccess,
            ErrorMessage = grpcResponse.ErrorMessage,
            Id = grpcResponse.Id
        };
    }
}