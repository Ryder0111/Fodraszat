using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string email, string password);
    }
}
