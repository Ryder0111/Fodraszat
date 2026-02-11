using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Services.Interfaces;

namespace FodraszatIdopont.Services
{
    public class UserRepository : IUserRepository
    {
        public Task Add(User felhasznalo)
        {
            throw new NotImplementedException();
        }

        public Task Delete(User felhasznalo)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetUserByEamil(string eamil)
        {
            throw new NotImplementedException();
        }

        public Task Update(User felhasznalo)
        {
            throw new NotImplementedException();
        }
    }
}
