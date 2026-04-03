using FodraszatIdopont.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FodraszatIdopont.Controllers
{
    [Authorize(Roles = "Hairdresser")]
    public class HairdresserController : Controller
    {
        public readonly IAppointmentService _AppointmentService;
        public readonly ICurrentUserService _CurrentUserService;

        public HairdresserController(IAppointmentService appointmentService, ICurrentUserService currentUserService)
        {
            _AppointmentService = appointmentService;
            _CurrentUserService = currentUserService;
        }

        public IActionResult Index()
        {
            return View("Indexh");
        }

        public async Task<IActionResult> Appointments(int offset = 0)
        {
            if (offset < 0) offset = 0;
            if (offset > 6) offset = 6;

            var idopontok = await _AppointmentService.GetHairdresserSchedule(_CurrentUserService.UserId,offset);
            if (!idopontok.Success)
            {
                TempData["error_msg"] = idopontok.Error;
                return View();
            }

            ViewBag.CurrentOffset = offset;
            ViewBag.TargetDate = DateOnly.FromDateTime(DateTime.Now.AddDays(offset));

            return View(idopontok.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var idopont = await _AppointmentService.CompleteAppointment(id);
            if (!idopont.Success)
            {
                TempData["error_msg"] = idopont.Error;
                return RedirectToAction("Appointments");
            }

            TempData["msg"] = "Befejezve";
            return RedirectToAction("Appointments");
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointmentStaff(int id)
        {
            var idopontok = await _AppointmentService.CancelAppointment(id);
            if (!idopontok.Success)
            {
                TempData["error_msg"] = idopontok.Error;
                return RedirectToAction("Appointments");
            }

            TempData["msg"] = "Lemondva";
            return RedirectToAction("Appointments");
        }
    }
}
