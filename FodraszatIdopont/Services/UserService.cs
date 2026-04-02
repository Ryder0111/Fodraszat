using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services.Interface;

namespace FodraszatIdopont.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Results<List<User>>> GetAllUsers()
        {
            var users = await _userRepository.GetAll();
            if(!users.Any())
            {
                return Results<List<User>>.Fail("Nincsenek még felhasználók");
            }
            return Results<List<User>>.Ok(users);
        }

        public async Task<Results<User>> GetUserById(int id)
        {
            var user = await _userRepository.GetById(id);
            if(user == null)
            {
                return Results<User>.Fail("Nincs ilyen felhasználó");
            }
            return Results<User>.Ok(user);
        }

        public async Task<Results<User>> UpdateUser(User user)
        {
            var existingUser = await _userRepository.GetById(user.UserId);

            if(existingUser == null)
            {
                return Results<User>.Fail("Nincs ilyen felhasználó");
            }

            await _userRepository.Update(user);
            return Results<User>.Ok(existingUser);
        }
    }
}
