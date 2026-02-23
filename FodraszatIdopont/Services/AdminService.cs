using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.Enums;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services.Interface;

namespace FodraszatIdopont.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _UserRepo;
        private readonly IServiceRepository _ServiceRepo;

        public AdminService(IUserRepository repo1, IServiceRepository repo2)
        {
            _UserRepo = repo1;
            _ServiceRepo = repo2;
        }

        public async Task<Results<Service>> CreateService(Service service)
        {
            if (service.Name != "")
                return Results<Service>.Fail("A név megadása kötelező");

            if (!(service.DurationInMinute > 0))
                return Results<Service>.Fail("Az időtartam megadása kötelező");

            if (!(service.Price > 0))
                return Results<Service>.Fail("Az ér megadása kötelező");

            _ServiceRepo.


        }

        public async Task<Results<User>> PromoteToHairdresser(string email)
        {
            var fodrasz = await _UserRepo.GetUserByEamil(email);
            if (fodrasz == null)
                return Results<User>.Fail("Nincs ilyen felhasználó");

            else if(fodrasz.Role.HasFlag(UserRole.Hairdresser))
                return Results<User>.Fail("A felhasználó már fodrász");

            else
                fodrasz.Role |= UserRole.Hairdresser;

            await _UserRepo.Update(fodrasz);
            return Results<User>.Ok(fodrasz);
        }

        public async Task<Results<User>> RemoveHairdresserRole(string email)
        {
            var fodrasz = await _UserRepo.GetUserByEamil(email);
            if (fodrasz == null)
                return Results<User>.Fail("Nincs ilyen felhasználó");

            else if (!fodrasz.Role.HasFlag(UserRole.Hairdresser))
                return Results<User>.Fail("A felhasználó még fodrász");

            else
                fodrasz.Role &= UserRole.Hairdresser;

            await _UserRepo.Update(fodrasz);
            return Results<User>.Ok(fodrasz);
        }

        public Task<Results<Service>> UpdateService(int serviceid)
        {
            throw new NotImplementedException();
        }
    }
}
