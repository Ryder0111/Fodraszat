namespace FodraszatIdopont.Models.AuthService
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string email, string password);
    }
}
