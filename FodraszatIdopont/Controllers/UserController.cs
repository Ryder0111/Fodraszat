using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FodraszatIdopont.Controllers
{
    public class UserController : Controller
    {

        public readonly IAppointmentService _AppointmentService;
        public readonly ICurrentUserService _CurrentUserService;

        public UserController(IAppointmentService appointmentService, ICurrentUserService currentUserService)
        {
            _AppointmentService = appointmentService;
            _CurrentUserService = currentUserService;
        }

        public async Task<IActionResult> Index()
        {
            var idopontok = await _AppointmentService.GetUserAppointments(_CurrentUserService.UserId);
            if (!idopontok.Success)
            {
                TempData["error_msg"] = idopontok.Error;
                return View();
            }
            
            return View(idopontok.Data);
        }
    }
}
