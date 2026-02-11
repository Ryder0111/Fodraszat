using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();

        Task<User?> GetUserByEamil(string eamil);

        Task<User?> GetById(int id);

        Task Add(User felhasznalo);

        Task Delete(User felhasznalo);

        Task Update(User felhasznalo);

    }
}
