using FodraszatIdopont.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FodraszatIdopont.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        public AdminController(IUserService userService)
        {
            _userService = userService;
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
    }
}
