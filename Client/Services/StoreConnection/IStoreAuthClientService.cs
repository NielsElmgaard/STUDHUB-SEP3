using StudHub.SharedDTO.StoreCredentials;

namespace Client.Services.StoreConnection;

public interface IStoreAuthClientService
{
    Task<BrickLinkCredentialsResponseDTO> SetBrickLinkCredentials(
        BrickLinkCredentialsRequestDTO credentialsRequest);
    
    Task<BrickOwlCredentialsResponseDTO> SetBrickOwlCredentials(
        BrickOwlCredentialsRequestDTO credentialsRequest);
}