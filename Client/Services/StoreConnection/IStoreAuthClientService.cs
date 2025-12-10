using StudHub.SharedDTO;
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

    Task<ConnectionStatusDTO?> IsBrickLinkConnected(long studUserId);
    Task<ConnectionStatusDTO?> IsBrickOwlConnected(long studUserId);

}