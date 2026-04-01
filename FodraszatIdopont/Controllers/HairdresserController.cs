using FodraszatIdopont.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FodraszatIdopont.Controllers
{
    public class HairdresserController : Controller
    {
        public readonly IAppointmentService _AppointmentService;
        public readonly ICurrentUserService _CurrentUserService;

        public HairdresserController(IAppointmentService appointmentService, ICurrentUserService currentUserService)
        {
            _AppointmentService = appointmentService;
            _CurrentUserService = currentUserService;
        }

        [Authorize(Roles = "Hairdresser")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Hairdresser")]
        public async Task<IActionResult> Appointments()
        {
            var idopontok = await _AppointmentService.GetHairdresserSchedule(_CurrentUserService.UserId);
            if (!idopontok.Success)
            {
                TempData["error_msg"] = idopontok.Error;
                return View();
            }

            return View(idopontok.Data);
        }

        [HttpPost]
        [Authorize(Roles = "Hairdresser")]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var idopont = await _AppointmentService.CompleteAppointment(id);
            if (!idopont.Success)
            {
                TempData["error_msg"] = idopont.Error;
                return RedirectToAction("Appointments");
            }
            else
            {
                TempData["msg"] = "Befejezve";
                return RedirectToAction("Appointments");
            }
        }
    }
}
