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

        public async Task<Results<User>> AuthenticateAsync(string email, string password)
        {
            var user = await _user.GetUserByEamil(email);
            if (user != null)
            {
                if (PasswordHelper.VerifyPassword(password, user.PasswordHash))
                {
                    return Results<User>.Ok(user);
                }
            }
            return Results<User>.Fail("Hibás jelszó vagy email cím");
        }

        public async Task<Results<User>> RegisterAsync(User felhasznalo,string password)
        {
            var user = await _user.GetUserByEamil(felhasznalo.Email);
            if (user != null)
            {
                return Results<User>.Fail("Email már foglalt");
            }
            felhasznalo.PasswordHash = PasswordHelper.HashPassword(password);
            felhasznalo.Role = Models.Enums.UserRole.User;
            await _user.Add(felhasznalo);
            return Results<User>.Ok(felhasznalo);
            
        }
    }
}
