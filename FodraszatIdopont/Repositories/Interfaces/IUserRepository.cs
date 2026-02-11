using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();

        Task<User?> GetUserByEamil(string email);

        Task<User?> GetById(int id);

        Task Add(User felhasznalo);

        Task Delete(User felhasznalo);

        Task Update(User felhasznalo);

    }
}
