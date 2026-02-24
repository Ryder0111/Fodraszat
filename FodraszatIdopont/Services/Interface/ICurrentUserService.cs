using FodraszatIdopont.Models.Enums;

namespace FodraszatIdopont.Services.Interface
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        bool IsAuthenticated { get; }
        UserRole Roles { get; }
    }
}
