using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Repositories.Interfaces
{
    public interface IServiceRepository
    {
        Task<Service?> GetById(int id);

        Task<bool> ExistsByName(string name);

        Task<Service> Create(Service service);

        Task<Service> Update(Service service);

        Task<Service?> DeavtiveService(Service service);

        Task<List<Service>> GetAll();


    }
}
