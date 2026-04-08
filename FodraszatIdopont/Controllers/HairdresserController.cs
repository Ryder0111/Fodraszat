using FodraszatIdopont.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FodraszatIdopont.Controllers
{
    [Authorize(Roles = "Hairdresser")]
    public class HairdresserController : BaseController
    {
        public readonly IAppointmentService _AppointmentService;
        public readonly ICurrentUserService _CurrentUserService;
        private readonly IWebHostEnvironment _env;

        public HairdresserController(IAppointmentService appointmentService, ICurrentUserService currentUserService, IWebHostEnvironment env)
        {
            _AppointmentService = appointmentService;
            _CurrentUserService = currentUserService;
            _env = env;
        }

        public IActionResult Index()
        {
            return View("Indexh");
        }

        public async Task<IActionResult> Appointments(int offset)
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
        public async Task<IActionResult> CompleteAppointment(int id, int offset)
        {
            var idopont = await _AppointmentService.CompleteAppointment(id);
            if (!idopont.Success)
            {
                TempData["error_msg"] = idopont.Error;
                return RedirectToAction("Appointments", new { offset = offset });
            }

            TempData["msg"] = "Befejezve";
            return RedirectToAction("Appointments", new { offset = offset });
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

        [HttpPost]
        public async Task<IActionResult> CancelAllAppointments(int id, int offset)
        {
            var idopontok = await _AppointmentService.CancelAllAppointments(id, offset);
            if (!idopontok.Success)
            {
                TempData["error_msg"] = idopontok.Error;
                WriteToLog($"Hiba a tömeges lemondásnál: {idopontok.Error}", _env.ContentRootPath);
                return RedirectToAction("Appointments", new { offset = offset });
            }

            TempData["msg"] = "Befejezve";
            return RedirectToAction("Appointments", new { offset = offset });
        }
    }
}
