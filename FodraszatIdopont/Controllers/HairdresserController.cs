using FodraszatIdopont.Helpers;
using FodraszatIdopont.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FodraszatIdopont.Controllers
{
    [Authorize(Roles = "Hairdresser")]
    public class HairdresserController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ICurrentUserService _currentUserService;
        private readonly LoggerHelper _logger;

        public HairdresserController(IAppointmentService appointmentService, ICurrentUserService currentUserService, LoggerHelper logger)
        {
            _appointmentService = appointmentService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View("Indexh");
        }

        public async Task<IActionResult> Appointments(int offset)
        {
            if (offset < 0) offset = 0;
            if (offset > 6) offset = 6;

            var idopontok = await _appointmentService.GetHairdresserSchedule(_currentUserService.UserId,offset);
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
            var idopont = await _appointmentService.CompleteAppointment(id);
            if (!idopont.Success)
            {
                TempData["error_msg"] = idopont.Error;
                return RedirectToAction("Appointments", new { offset = offset });
            }

            TempData["msg"] = "Befejezve";
            _logger.Log("INFO", $"HairdresserId={_currentUserService.UserId} Appointment completed (Id={id})");
            return RedirectToAction("Appointments", new { offset = offset });
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointmentStaff(int id)
        {
            var idopontok = await _appointmentService.CancelAppointment(id);
            if (!idopontok.Success)
            {
                TempData["error_msg"] = idopontok.Error;
                return RedirectToAction("Appointments");
            }

            TempData["msg"] = "Lemondva";
            _logger.Log("INFO", $"HairdresserId={_currentUserService.UserId} Appointment cancel (Id={id})");
            return RedirectToAction("Appointments");
        }

        [HttpPost]
        public async Task<IActionResult> CancelAllAppointments(int id, int offset)
        {
            var idopontok = await _appointmentService.CancelAllAppointments(id, offset);
            if (!idopontok.Success)
            {
                TempData["error_msg"] = idopontok.Error;
                _logger.Log("ERROR",$"HairdresserId={_currentUserService.UserId} Bulk cancel failed (Error={idopontok.Error})");
                return RedirectToAction("Appointments", new { offset = offset });
            }

            TempData["msg"] = "Befejezve";
            return RedirectToAction("Appointments", new { offset = offset });
        }
    }
}
