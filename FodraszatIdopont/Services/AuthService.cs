using FodraszatIdopont.Data;
using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Repositories;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FodraszatIdopont.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _user;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(IUserRepository user, IHttpContextAccessor httpContextAccessor)
        {
            _user = user;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Results<User>> AuthenticateAsync(string email, string password)
        {
            var user = await _user.GetUserByEamil(email);
            if (user != null)
            {
                Console.WriteLine($"\n\n ---> BELÉPÉS TESZT: Keresett email: {email} | Megtalált ID: {user.UserId} | Verified: {user.IsEmailVerified} | Token maradt: {user.EmailVerificationToken ?? "NULL"} <---\n\n");
                if (user.IsEmailVerified)
                {
                    if (PasswordHelper.VerifyPassword(password, user.PasswordHash))
                    {
                        return Results<User>.Ok(user);
                    }
                }
                else
                {
                    return Results<User>.Fail("Előbb meg kell erősítened az email címed!");
                }
            }
            return Results<User>.Fail("Hibás jelszó vagy email cím");
        }

        public async Task<Results<User>> RegisterAsync(User felhasznalo,string password)
        {
            var user = await _user.GetUserByEamil(felhasznalo.Email);
            if (user != null)
            {
                return Results<User>.Fail("Ez az email cím már foglalt!");
            }
            felhasznalo.PasswordHash = PasswordHelper.HashPassword(password);
            felhasznalo.Role = Models.Enums.UserRole.User;
            await _user.Add(felhasznalo);
            return Results<User>.Ok(felhasznalo);
        }

        public async Task SignInUserAsync(User user, bool rememberMe)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if(httpContext==null) return;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.Email,user.Email),
            };

            if (user.Role.HasFlag(Models.Enums.UserRole.Admin))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            if (user.Role.HasFlag(Models.Enums.UserRole.Hairdresser))
            {
                claims.Add(new Claim(ClaimTypes.Role, "Hairdresser"));
            }

            if (user.Role.HasFlag(Models.Enums.UserRole.User))
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role);

            var principal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,

                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null,
                IssuedUtc = rememberMe ? DateTimeOffset.UtcNow : null
            };

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties
            );
        }

        public async Task<Results<string>> VerifyByToken(string verificationToken)
        {
            if (string.IsNullOrWhiteSpace(verificationToken))
            {
                return Results<string>.Fail("Érvénytelen link: Hiányzik a token!");
            }

            var validate = await _user.GetByToken(verificationToken);

            if (validate == null)
            {
                return Results<string>.Fail("Érvénytelen vagy lejárt token!");
            }

            validate.IsEmailVerified = true;
            validate.EmailVerificationToken = null;
            try
            {
                // Megpróbáljuk menteni az adatbázisba
                await _user.Update(validate);
                Console.WriteLine($"\n\n ---> Mentés TESZT: Keresett email: {validate.Email} | Megtalált ID: {validate.UserId} | Verified: {validate.IsEmailVerified} | Token maradt: {validate.EmailVerificationToken ?? "NULL"} <---\n\n");
                return Results<string>.Ok("Sikeres email megerősítés!");
            }
            catch (Exception ex)
            {
                // HA IGAZÁBÓL NEM MENTI EL, ITT FOG KIDERÜLNI A VALÓDI OK!
                return Results<string>.Fail($"Adatbázis hiba: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}
