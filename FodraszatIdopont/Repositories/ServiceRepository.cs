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

        public async Task<Service> Ceate(Service service)
        {
            _db.Services.Add(service);
            await _db.SaveChangesAsync();
            return service;
        }

        public async Task<Service?> DeavtiveService(Service service)
        {
            var szolgaltatas = await _db.Services.FirstOrDefaultAsync(s=>s.ServiceId==service.ServiceId);

            szolgaltatas.isActive = false;

            await _db.SaveChangesAsync();
            return szolgaltatas;

        }

        public async Task<bool> ExistsByName(string name)
        {
            return await _db.Services.AnyAsync(s => s.Name == name);
        }

        public async Task<List<Service>> GetAll()
        {
            return await _db.Services.ToListAsync();
        }

        public async Task<Service?> GetById(int id)
        {
            return await _db.Services.SingleOrDefaultAsync(s => s.ServiceId == id);
        }

        public async Task<Service> Update(Service service)
        {
            _db.Services.Update(service);
            await _db.SaveChangesAsync();
            return service;
        }

    }
}
