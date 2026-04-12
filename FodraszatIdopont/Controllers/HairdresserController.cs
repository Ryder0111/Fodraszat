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
        private readonly IEmailService _emailService;

        private string BaseUrl
        {
            get
            {
                return $"{Request.Scheme}://{Request.Host}";
            }
        }

        public HairdresserController(IAppointmentService appointmentService, ICurrentUserService currentUserService, LoggerHelper logger, IEmailService emailService)
        {
            _appointmentService = appointmentService;
            _currentUserService = currentUserService;
            _logger = logger;
            _emailService = emailService;   
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
            var idopont = await _appointmentService.CancelAppointment(id);
            if (!idopont.Success)
            {
                TempData["error_msg"] = idopont.Error;
                return RedirectToAction("Appointments");
            }

            TempData["msg"] = "Lemondva";

            string subject = "Időpont törlése a Wild Cut Fodrászatnál";
            string message = $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <h3 style='color: #4b2c61;'>Kedves {idopont.Data!.User!.Name}!</h3>
                    <p>Sajnálattal értesítünk, hogy a(z) <strong>{idopont.Data!.StartTime.ToString("yyyy. MM. dd. HH:mm")}</strong> időpontra szóló foglalásodat fodrászod, <strong>{idopont.Data!.Hairdresser!.Name}</strong> váratlan okok miatt lemondta.</p>
                    <p>Elnézést kérünk az okozott kellemetlenségért! Reméljük, hamarosan újra vendégeink között tudhatunk. Kérjük, látogass el weboldalunkra, és foglalj egy új időpontot.</p>
                    <br/>
                    <a href='{BaseUrl}' style='display: inline-block; padding: 12px 20px; background-color: #4b2c61; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>Új időpont foglalása</a>
                </div>";

            await _emailService.SendEmailAsync(idopont.Data!.User!.Email,subject,message);

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

            string subject;
            string message;

            foreach (var idopont in idopontok.Data!)
            {
                subject = "Időpont törlése a Wild Cut Fodrászatnál";
                message = $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <h3 style='color: #4b2c61;'>Kedves {idopont.User!.Name}!</h3>
                    <p>Sajnálattal értesítünk, hogy a(z) <strong>{idopont.StartTime.ToString("yyyy. MM. dd. HH:mm")}</strong> időpontra szóló foglalásodat fodrászod, <strong>{idopont.Hairdresser!.Name}</strong> váratlan okok miatt lemondta.</p>
                    <p>Elnézést kérünk az okozott kellemetlenségért! Reméljük, hamarosan újra vendégeink között tudhatunk. Kérjük, látogass el weboldalunkra, és foglalj egy új időpontot.</p>
                    <br/>
                    <a href='{BaseUrl}' style='display: inline-block; padding: 12px 20px; background-color: #4b2c61; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>Új időpont foglalása</a>
                </div>";

                await _emailService.SendEmailAsync(idopont.User!.Email, subject, message);
            }

            TempData["msg"] = "Befejezve";
            return RedirectToAction("Appointments", new { offset = offset });
        }
    }
}
