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
            if (service == null)
                return Results<Service>.Fail("Érvénytelen szolgáltatás adat!");

            if (await _ServiceRepo.ExistsByName(service.Name))
                return Results<Service>.Fail("Ez a szolgáltatás már létezik!");

                await _ServiceRepo.Create(service);
                return Results<Service>.Ok(service);
        }

        public async Task<Results<User>> PromoteToHairdresser(string email)
        {

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
            var fodrasz = await _UserRepo.GetUserByEamil(email);
            if (fodrasz == null)
                return Results<User>.Fail("Nincs ilyen felhasználó");

            if (!fodrasz.Role.HasFlag(UserRole.Hairdresser))
                return Results<User>.Fail("A felhasználó még fodrász");

            fodrasz.Role &= ~UserRole.Hairdresser;

            await _UserRepo.Update(fodrasz);
            return Results<User>.Ok(fodrasz);
        }

        public async Task<Results<Service>> UpdateService(Service service)
        {
            if (service == null)
                return Results<Service>.Fail("Érvénytelen szolgáltatás adat!");

            var szolgaltatas = await _ServiceRepo.GetById(service.ServiceId);
            if (szolgaltatas == null)
                return Results<Service>.Fail("Nincs ilyen szolgáltatás!");

            if (await _ServiceRepo.ExistsByNameExceptId(service.Name, service.ServiceId))
                return Results<Service>.Fail("Ez a szolgáltatás már létezik!");

            return Results<Service>.Ok(await _ServiceRepo.Update(service));

        }
    }
}
