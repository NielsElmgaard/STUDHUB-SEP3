using StudHub.SharedDTO.Lager;

namespace Studhub.AppServer.Services.Lager;

public class StudhubLagerService
{
    private readonly StudhubLagerRepository _repository;

    public StudhubLagerService(StudhubLagerRepository repository)
    {
        _repository = repository;
    }

    
    public async Task<List<LagerItemDTO>> HentLagerOversigtAsync()
    {
        return await _repository.GetAllItemsAsync();
    }

    
    public async Task<LagerDetaljerDTO?> HentElementDetaljerAsync(int id)
    {
        return await _repository.GetItemDetailsAsync(id);
    }

    
    public async Task<List<LagerItemDTO>> SøgOgFiltrerAsync(
        string? searchText,
        string? color,
        string? partId,
        string? platform)
    {
        return await _repository.SearchAsync(
            searchText,
            color,
            partId,
            platform);
    }
}