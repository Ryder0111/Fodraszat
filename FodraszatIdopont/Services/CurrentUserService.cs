using FodraszatIdopont.Models.Enums;
using FodraszatIdopont.Services.Interface;
using System.Security.Claims;

namespace FodraszatIdopont.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _Http;

        public CurrentUserService(IHttpContextAccessor http)
        {
            _Http = http;
        }

        public int? UserId
        {
            get
            {
                var user = _Http.HttpContext?.User;

                if (user == null || !user.Identity?.IsAuthenticated == true)
                    return null;

                var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);

                if (idClaim == null)
                    return null;

                return int.Parse(idClaim.Value);
            }
        }

        public bool IsAuthenticated =>
            _Http.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public UserRole Roles
        {
            get
            {
                var user = _Http.HttpContext?.User;

                if (user == null || !user.Identity?.IsAuthenticated == true)
                    return UserRole.None;

                var roleClaim = user.FindFirst(ClaimTypes.Role);

                if (roleClaim == null)
                    return UserRole.None;

                return Enum.Parse<UserRole>(roleClaim.Value);
            }
        }
    }
}
