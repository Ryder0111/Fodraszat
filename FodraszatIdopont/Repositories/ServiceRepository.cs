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

        public async Task<Service?> GetServiceById(int id)
        {
            return await _db.Services.SingleOrDefaultAsync(s => s.ServiceId == id);
        }

        public async Task<List<Service>?> GetAllService()
        {
            return await _db.Services.ToListAsync();
        }

    }
}
