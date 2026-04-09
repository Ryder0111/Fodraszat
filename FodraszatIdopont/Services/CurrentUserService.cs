using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.Enums;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services.Interface;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace FodraszatIdopont.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _Http;
        private readonly IUserRepository _userRepo;

        public CurrentUserService(IHttpContextAccessor http,IUserRepository userRepository)
        {
            _Http = http;
            _userRepo = userRepository;
        }

        public int? UserId
        {
            get
            {
                var user = _Http.HttpContext?.User;

                if (user?.Identity?.IsAuthenticated != true)
                    return null;

                var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);

                if (idClaim == null)
                    return null;

                if (int.TryParse(idClaim.Value, out int userId))
                    return userId;

                return null;
            }
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            if(UserId == null) return null;

            var user = await _userRepo.GetById(UserId.Value);
            return user;
        }

        public bool IsAuthenticated =>
            _Http.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public UserRole Roles
        {
            get
            {
                var user = _Http.HttpContext?.User;

                if (user == null || user.Identity?.IsAuthenticated != true)
                    return UserRole.None;

                var roleClaims = user.FindAll(ClaimTypes.Role);

                if (!roleClaims.Any())
                    return UserRole.None;

                UserRole combinedRoles = UserRole.None;

                foreach (var claim in roleClaims)
                {
                    if (Enum.TryParse<UserRole>(claim.Value, out var parsedRole))
                    {
                        combinedRoles |= parsedRole;
                    }
                }

                return combinedRoles;
            }
        }
    }
}
