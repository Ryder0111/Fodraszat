using FodraszatIdopont.Data;
using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FodraszatIdopont.Services
{
    public class AuthService : IAuthService
    {
        private readonly BarberDbContext _db;
        public AuthService(BarberDbContext barberDb)
        {
            _db = barberDb;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                if (PasswordHelper.VerifyPassword(password, user.PasswordHash))
                {
                    return user;
                }
                return null;
            }
            return null;
        }
    }
}
