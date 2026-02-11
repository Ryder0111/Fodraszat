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
            throw new NotImplementedException();
        }

        public Task Delete(User felhasznalo)
        {
            throw new NotImplementedException();
        }

        public Task Update(User felhasznalo)
        {
            throw new NotImplementedException();
        }
    }
}
