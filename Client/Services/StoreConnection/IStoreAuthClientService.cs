using StudHub.SharedDTO.StoreCredentials;

namespace Client.Services.StoreConnection;

public interface IStoreAuthClientService
{
    Task<BrickLinkCredentialsResponseDTO> SetBrickLinkCredentials(
        BrickLinkCredentialsRequestDTO credentialsRequest);
    
    Task<BrickOwlCredentialsResponseDTO> SetBrickOwlCredentials(
        BrickOwlCredentialsRequestDTO credentialsRequest);
    
    Task<BrickLinkCredentialsResponseDTO> ClearBrickLinkCredentials(long studUserId);
    
    Task<BrickOwlCredentialsResponseDTO> ClearBrickOwlCredentials(long studUserId);

    Task<bool> IsBrickLinkConnected(long studUserId);
    Task<bool> IsBrickOwlConnected(long studUserId);

}