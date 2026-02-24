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
        private readonly ICurrentUserService _CurrentUser;

        public AdminService(IUserRepository repo1, IServiceRepository repo2, ICurrentUserService repo3)
        {
            _UserRepo = repo1;
            _ServiceRepo = repo2;
            _CurrentUser = repo3;
        }

        public async Task<Results<Service>> CreateService(Service service)
        {

            if (!_CurrentUser.IsAuthenticated)
                return Results<Service>.Fail("Nincs hozzá jogosultságod");

            if (!_CurrentUser.Roles.HasFlag(UserRole.Admin))
                return Results<Service>.Fail("Nincs hozzá jogosultságod");

            if (string.IsNullOrWhiteSpace(service.Name))
                return Results<Service>.Fail("A név megadása kötelező");

            if (service.DurationInMinute <= 0)
                return Results<Service>.Fail("Az időtartam megadása kötelező");

            if (service.Price <= 0)
                return Results<Service>.Fail("Az ér megadása kötelező");

            if (await _ServiceRepo.ExistsByName(service.Name))
            {
                return Results<Service>.Fail("Ez a szolgáltatás már létezik!");
            }

                await _ServiceRepo.Ceate(service);
                return Results<Service>.Ok(service);
        }

        public async Task<Results<User>> PromoteToHairdresser(string email)
        {
            if (!_CurrentUser.IsAuthenticated)
                return Results<User>.Fail("Nincs hozzá jogosultságod");

            if (!_CurrentUser.Roles.HasFlag(UserRole.Admin))
                return Results<User>.Fail("Nincs hozzá jogosultságod");

            var fodrasz = await _UserRepo.GetUserByEamil(email);
            if (fodrasz == null)
                return Results<User>.Fail("Nincs ilyen felhasználó");

            if(fodrasz.Role.HasFlag(UserRole.Hairdresser))
                return Results<User>.Fail("A felhasználó már fodrász");

            fodrasz.Role |= UserRole.Hairdresser;

            await _UserRepo.Update(fodrasz);
            return Results<User>.Ok(fodrasz);
        }

        public async Task<Results<User>> RemoveHairdresserRole(string email)
        {
            if (!_CurrentUser.IsAuthenticated)
                return Results<User>.Fail("Nincs hozzá jogosultságod");

            if (!_CurrentUser.Roles.HasFlag(UserRole.Admin))
                return Results<User>.Fail("Nincs hozzá jogosultságod");

            var fodrasz = await _UserRepo.GetUserByEamil(email);
            if (fodrasz == null)
                return Results<User>.Fail("Nincs ilyen felhasználó");

            else if (!fodrasz.Role.HasFlag(UserRole.Hairdresser))
                return Results<User>.Fail("A felhasználó még fodrász");

            else
                fodrasz.Role &= ~UserRole.Hairdresser;

            await _UserRepo.Update(fodrasz);
            return Results<User>.Ok(fodrasz);
        }

        public async Task<Results<Service>> UpdateService(Service service)
        {
            if (!_CurrentUser.IsAuthenticated)
                return Results<Service>.Fail("Nincs hozzá jogosultságod");

            if (!_CurrentUser.Roles.HasFlag(UserRole.Admin))
                return Results<Service>.Fail("Nincs hozzá jogosultságod");

            var szolgaltatas = await _ServiceRepo.GetById(service.ServiceId);
            if (szolgaltatas == null)
            {
                return Results<Service>.Fail("Nincs ilyen szolgáltatás!");
            }
            if (service.Price <= 0)
            {
                return Results<Service>.Fail("A szolgáltatás ára hibásan lett megadva!");
            }
            if (service.DurationInMinute <= 0)
            {
                return Results<Service>.Fail("A szolgáltatás időtartama nem megfelelő!");
            }

            return Results<Service>.Ok(await _ServiceRepo.Update(service));

        }
    }
}
