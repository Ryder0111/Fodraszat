using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Services.Interface
{
    public interface IAdminService
    {
        Task<Results<User>> PromoteToHairdresser(string email);
        Task<Results<User>> RemoveHairdresserRole(string email);

        Task<Results<Service>> CreateService(Service service);

        Task<Results<Service>> UpdateService(int serviceid);
    }
}
