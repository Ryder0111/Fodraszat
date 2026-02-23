using FodraszatIdopont.Data;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FodraszatIdopont.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly BarberDbContext _db;

        public ServiceRepository(BarberDbContext db)
        {
            _db = db;
        }

        public Task<Service> Ceate(Service service)
        {
            throw new NotImplementedException();
        }

        public Task<Service> DeavtiveService(Service service)
        {
            throw new NotImplementedException();
        }

        public async Task<Service?> ExistsByName(string name)
        {
            return await _db.Services.SingleOrDefaultAsync(s => s.Name == name);
        }

        public Task<List<Service>> GetAll(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Service?> GetById(int id)
        {
            return await _db.Services.SingleOrDefaultAsync(s => s.ServiceId == id);
        }



        public Task<Service> Update(Service service)
        {
            throw new NotImplementedException();
        }
    }
}
