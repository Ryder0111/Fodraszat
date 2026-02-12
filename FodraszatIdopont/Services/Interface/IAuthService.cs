using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Services.Interface
{
    public interface IAuthService
    {
        Task<Results<User>> AuthenticateAsync(string email,string password);

        Task<Results<User>> RegisterAsync(User felhasznalo, string password);
    }
}
