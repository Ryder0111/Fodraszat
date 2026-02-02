using Microsoft.AspNetCore.Mvc;

namespace FodraszatIdopont.Images
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
