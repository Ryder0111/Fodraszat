using Microsoft.AspNetCore.Mvc;

namespace FodraszatIdopont.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
