using FodraszatIdopont.Data;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FodraszatIdopont.Repositories
{
    public class ClosedDayRepository : IClosedDayRepository
    {
        private readonly BarberDbContext _db;

        public ClosedDayRepository(BarberDbContext db)
        {
            _db = db;
        }

        public async Task<ClosedDay> Add(ClosedDay closedday)
        {
            _db.Add(closedday);
            await  _db.SaveChangesAsync();
            return closedday;
        }

        public async Task<bool> ExistsByDate(DateOnly closedday)
        {
            return await _db.ClosedDays.AnyAsync(d => d.Date == closedday);
        }

        public async Task<List<ClosedDay>> GettAll()
        {
            return await _db.ClosedDays.ToListAsync();
        }

        public async Task<DateOnly> DeleteByDate(DateOnly closedday)
        {
            _db.ClosedDays.Remove(await _db.ClosedDays.FirstAsync(d => d.Date == closedday));
            await _db.SaveChangesAsync();
            return closedday;
        }
    }
}
