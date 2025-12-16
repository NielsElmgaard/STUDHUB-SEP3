using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;
using StudHub.SharedDTO.Lager;

namespace Studhub.AppServer.Services.Lager;

public interface IStudhubLagerService
{
    public Task<List<LagerItemDTO>> HentLagerOversigtAsync();
    Task<LagerDetaljerDTO?> HentElementDetaljerAsync(int id);
}