using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Repositories.Interfaces
{
    public interface IServiceRepository
    {
        Task<Service?> GetServiceById(int id);
        Task<List<Service>?> GetAllService();
    }
}
