using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.Enums;
using System.Threading.Tasks;

namespace FodraszatIdopont.Services.Interface
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        Task<User?> GetCurrentUserAsync();
        bool IsAuthenticated { get; }
        UserRole Roles { get; }
    }
}
