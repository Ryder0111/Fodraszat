using FodraszatIdopont.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FodraszatIdopont.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAdminService _adminService;
        public AdminController(IUserService userService, IAdminService adminService)
        {
            _userService = userService;
            _adminService = adminService;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var response = await _userService.GetAllUsers();
            if(!response.Success)
            {
                TempData["error_msg"] = response.Error;
                return RedirectToAction("Index");
            }
            return View(response.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _userService.GetUserById(id);
            if (!response.Success)
            {
                TempData["error_msg"] = response.Error;
                return RedirectToAction("Index");
            }
            return View(response.Data);
        }

        public async Task<IActionResult> ChangeRole(string userEmail, Models.Enums.UserRole newRole)
        {
            if(newRole == Models.Enums.UserRole.Hairdresser)
            {
                var response = await _adminService.PromoteToHairdresser(userEmail);
                if (!response.Success)
                {
                    TempData["error_msg"] = response.Error;
                    return RedirectToAction("Users");
                }
                TempData["msg"] = $"{response.Data!.Name} mostmár fodrász!";
                return RedirectToAction("Details", new { id = response.Data.UserId });
            }
            else
            {
                var response = await _adminService.RemoveHairdresserRole(userEmail);
                if (!response.Success)
                {
                    TempData["error_msg"] = response.Error;
                    return RedirectToAction("Users");
                }
                TempData["msg"] = $"{response.Data!.Name} mostmár csak vendég!";
                return RedirectToAction("Details", new { id = response.Data.UserId });
            }
        }
    }
}
