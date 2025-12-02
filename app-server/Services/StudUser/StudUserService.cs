using Grpc.Core;
using Microsoft.AspNetCore.Http.HttpResults;
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

    public async Task<CreateStudUserResponseDTO> CreateStudUser(
        CreateStudUserRequestDTO userRequest)
    {
        try
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
        catch (RpcException ex) when (ex.Status.StatusCode ==
                                      StatusCode.AlreadyExists ||
                                      ex.Status.StatusCode ==
                                      StatusCode.Unknown)
        {
            string detailMessage = ex.Status.Detail;
            string userMessage = detailMessage.Contains("A stud user")
                ? detailMessage
                : "A user with this email already exists.";

            return new CreateStudUserResponseDTO
            {
                IsSuccess = false,
                ErrorMessage = userMessage,
                Id = 0
            };
        }
        catch (Exception e)
        {
            return new CreateStudUserResponseDTO
            {
                IsSuccess = false,
                ErrorMessage =
                    $"An unexpected internal server error occurred: {e.Message}",
                Id = 0
            };
        }
    }
}