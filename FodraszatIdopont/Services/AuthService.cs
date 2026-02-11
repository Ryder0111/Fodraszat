using FodraszatIdopont.Data;
using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Repositories;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FodraszatIdopont.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _user;
        public AuthService(IUserRepository user)
        {
            _user = user;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _user.GetUserByEamil(email);
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
