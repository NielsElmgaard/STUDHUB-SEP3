using StudHub.SharedDTO;

namespace Studhub.AppServer.Services;

public interface IInventoryService
{
    Task<List<SetDTO>> GetUserSetsAsync(string userId);
}