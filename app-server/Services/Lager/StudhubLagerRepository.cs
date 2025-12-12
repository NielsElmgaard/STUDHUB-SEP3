using StudHub.SharedDTO.Lager;

namespace Studhub.AppServer.Services.Lager;

public class StudhubLagerRepository
{
    public Task<List<LagerItemDTO>> GetAllItemsAsync()
    {
        var items = new List<LagerItemDTO>
        {
            new LagerItemDTO
            {
                Id = 1,
                Name = "2x4 Brick"
            }
        };

        return Task.FromResult(items);
    }

    public Task<LagerDetaljerDTO?> GetItemDetailsAsync(int id)
    {
        if (id == 1)
        {
            var test = new LagerDetaljerDTO
            {
                Id = 1,
                Name = "2x4 Brick",
                Description = "Klassisk LEGO 2x4 klods i rød farve."
            };

            return Task.FromResult<LagerDetaljerDTO?>(test);
        }

        return Task.FromResult<LagerDetaljerDTO?>(null);
    }
    

    
    public Task<List<LagerItemDTO>> SearchAsync(
        string? searchText,
        string? color,
        string? partId,
        string? platform)
    {
        
        return Task.FromResult(new List<LagerItemDTO>());
    }
}   