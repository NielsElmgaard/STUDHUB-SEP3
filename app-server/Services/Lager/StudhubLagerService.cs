using System.Globalization;
using System.Text.Json;
using Studhub.Grpc.Data;
using StudHub.SharedDTO;
using StudHub.SharedDTO.Inventory;
using StudHub.SharedDTO.Lager;

namespace Studhub.AppServer.Services.Lager;

public class StudhubLagerService : IStudhubLagerService
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
}