using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Services.Interface
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string email,string password);
    }
}
