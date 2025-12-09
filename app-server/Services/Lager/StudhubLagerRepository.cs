using StudHub.SharedDTO.Lager;

namespace Studhub.AppServer.Services.Lager;

public class StudhubLagerRepository
{
    public Task<List<LagerItemDTO>> GetAllItemsAsync()
    {
        // Database implementeres af din ven
        return Task.FromResult(new List<LagerItemDTO>());
    }

    public Task<LagerDetaljerDTO?> GetItemDetailsAsync(int id)
    {
        // Database implementeres af din ven
        return Task.FromResult<LagerDetaljerDTO?>(null);
    }
}