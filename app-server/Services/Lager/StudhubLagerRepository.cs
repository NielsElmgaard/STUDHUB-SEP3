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
                Name = "LEGO sut"
            }
        };

        return Task.FromResult(items);
        // Database implementeres af din ven
        //return Task.FromResult(new List<LagerItemDTO>());
    }

    public Task<LagerDetaljerDTO?> GetItemDetailsAsync(int id)
    {
        if (id == 1)
        {
            var test = new LagerDetaljerDTO 
            {
                Id = 1,
                Name = "LEGO sut",
                Description = "LEGO Sutten kan bruges af alle. Savl sælges separat."
            };
            return Task.FromResult<LagerDetaljerDTO?>(test);
        }
        // Database implementeres af din ven
        return Task.FromResult<LagerDetaljerDTO?>(null);
    }
}