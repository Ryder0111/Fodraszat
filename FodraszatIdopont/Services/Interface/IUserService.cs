using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Services.Interface
{
    public interface IUserService
    {
        Task<Results<List<User>>> GetAllUsers();
        Task<Results<User>> GetUserById(int id);
        Task<Results<User>> UpdateUser(User user);
    }
}
