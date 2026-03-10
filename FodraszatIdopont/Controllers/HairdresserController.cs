using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FodraszatIdopont.Controllers
{
    public class HairdresserController : Controller
    {
        [Authorize(Roles = "Hairdresser")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
