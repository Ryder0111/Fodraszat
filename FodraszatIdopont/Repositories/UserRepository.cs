using FodraszatIdopont.Data;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FodraszatIdopont.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BarberDbContext _db;

        public UserRepository(BarberDbContext db)
        {
            _db = db;
        }

        public async Task<List<User>> GetAll()
        {
            return await _db.Users.OrderBy(x => x.Name).ToListAsync();
        }
        public async Task<List<User>> GetAllHairdresser()
        {
            return await _db.Users.Where(x => x.Role.HasFlag(Models.Enums.UserRole.Hairdresser)).OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<User?> GetById(int id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEamil(string email)
        {
            return await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task Add(User felhasznalo)
        {
            _db.Add(felhasznalo);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(User felhasznalo)
        {
            _db.Remove(felhasznalo);
            await _db.SaveChangesAsync();
        }

        public async Task Update(User felhasznalo)
        {
            _db.Update(felhasznalo);
            await _db.SaveChangesAsync();
        }
        public async Task<List<User>?> GetAllHairdresser()
        {
            return await _db.Users.Where(h=>h.Role.HasFlag(Models.Enums.UserRole.Hairdresser)).ToListAsync();
        }
    }
}
