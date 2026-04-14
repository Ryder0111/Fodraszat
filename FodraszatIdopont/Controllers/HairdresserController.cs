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
            <div style='max-width: 600px; margin: 0 auto; font-family: ""Segoe UI"", Arial, sans-serif; color: #2C2C2C; border: 1px solid #eee; border-radius:  10px;        overflow: hidden; background-color: #ffffff;'>
                <div style='background-color: #4A3018; padding: 25px; text-align: center;'>
                    <h2 style='color: #FDFBF7; margin: 0; font-size: 22px;'>Időpont lemondva</h2>
                </div>
                <div style='padding: 30px; line-height: 1.6;'>
                    <h3 style='color: #4A3018;'>Kedves {idopont.Data!.User!.Name}!</h3>
                    <p>Sajnálattal értesítünk, hogy a lenti időpontra szóló foglalásodat fodrászod, <strong>{idopont.Data!.Hairdresser!.Name}</strong> váratlan     okok    miatt  kénytelen volt lemondani.</p>
                    
                    <div style='background-color: #FDFBF7; border-left: 4px solid #B89151; padding: 15px; margin: 20px 0;'>
                        <p style='margin: 0;'><strong>Érintett időpont:</strong> <span style='color: #d9534f; font-weight: bold;'>{idopont.Data!.StartTime.ToString     ("yyyy.  MM. dd. HH:mm")}</span></p>
                    </div>
                    
                    <p>Elnézést kérünk az okozott kellemetlenségért! Reméljük, hamarosan újra a vendégeink között tudhatunk. Kérjük, látogass el weboldalunkra, és      foglalj  egy új időpontot.</p>

                    <table width='100%' border='0' cellspacing='0' cellpadding='0' style='margin-top: 30px; margin-bottom: 30px;'>
                        <tr>
                            <td align='center'>
                                <table border='0' cellspacing='0' cellpadding='0'>
                                    <tr>
                                        <td align='center' bgcolor='#B89151' style='padding: 14px 25px; border-radius: 5px;'>
                                            <a href='{BaseUrl}/Account/MAAppointment' target='_blank' style='font-family: Arial, sans-serif; fontsize: 16px;    text-decoration: none;font-weight: bold; display: block;'>
                                                <span style='color: #ffffff;'>Új időpont foglalása</span>
                                            </a>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 30px 0;' />
                    <p style='font-size: 13px; color: #666666; text-align: center;'>
                        Wild Cut Fodrászat - 3000 Hatvan, Kazinczy u. 3.
                    </p>
                </div>
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
