using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.Enums;
using FodraszatIdopont.Models.ViewModels;
using FodraszatIdopont.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FodraszatIdopont.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAppointmentService _appointService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;
        private readonly LoggerHelper _logger;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IAuthService authService, IAppointmentService appointService, ICurrentUserService currentUserService, IUserService userService, IEmailService emailService, LoggerHelper logger, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _appointService = appointService;
            _currentUserService = currentUserService;
            _userService = userService;
            _logger = logger;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }


        public IActionResult Login()
        {
            return View( new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //CSRF elleni védelem; CSRF-Cross-site request forgery
        public async Task<IActionResult> Login(LoginViewModel model, string recaptchaToken)
        {
            var isHuman = await VerifyRecaptcha(recaptchaToken);

            if (!isHuman)
            {
                ModelState.AddModelError("", "Robot ellenőrzés sikertelen.");
                _logger.Log("WARNING", $"Login bot detected Email={model.Email}");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                TempData["error_msg"] = "Próbáld újra!";
                return View(model);
            }

            var result = await _authService.AuthenticateAsync(model.Email, model.Password);

            if (!result.Success)
            {
                TempData["error_msg"] = result.Error ?? "Hibás email vagy jelszó";
                _logger.Log("WARNING", $"Login failed Email={model.Email}");
                return View(model);
            }


            var user = result.Data;
            await _authService.SignInUserAsync(user!, model.RememberMe);
            _logger.Log("INFO", $"UserId={user!.UserId} Login success");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Kijelentkeztetés a Cookie-ból
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Response.Cookies.Delete("FodraszatAuth");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.Log("INFO", $"UserId={userId} Logout");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(RegisterViewModel felhasznalo, string recaptchaToken)
        {
            var isHuman = await VerifyRecaptcha(recaptchaToken);

            if (!isHuman)
            {
                ModelState.AddModelError("", "Bot vagy!");
                return View(felhasznalo);
            }
;
            var verificationToken = Guid.NewGuid().ToString();

            if (!ModelState.IsValid) return View(model: felhasznalo);
            User user = new User()
            {
                Name = felhasznalo.Name,
                Phone = felhasznalo.Phone,
                Email = felhasznalo.Email,
                PasswordHash = PasswordHelper.HashPassword(felhasznalo.Password),
                Sex = felhasznalo.Sex,
                EmailVerificationToken = verificationToken,
                IsEmailVerified = false
            };
            var result = await _authService.RegisterAsync(user, felhasznalo.Password);
            if (!result.Success)
            {
                TempData["error_msg"] = result.Error;
                return View(felhasznalo);
            }

            var scheme = _httpContextAccessor.HttpContext!.Request.Scheme; // http vagy https
            var host = _httpContextAccessor.HttpContext.Request.Host;     // localhost:7294 vagy localhost:5001 stb.

            string baseUrl = $"{scheme}://{host}";
            string verifyUrl = $"{baseUrl}/Account/Verify/{verificationToken}";
            _logger.Log("INFO", $"UserId={user.UserId} Email={user.Email} Registration success");

            //------Email------
            string subject = "Üdvözöljük a Wild Cut csapatában!";
            string body = $@"
            <div style='max-width: 600px; margin: 0 auto; font-family: ""Segoe UI"", Arial, sans-serif; color: #333; border: 1px solid #eee; border-radius: 10px; overflow: hidden;'>
                <div style='background-color: #4b2c61; padding: 25px; text-align: center;'>
                    <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>Üdv a fedélzeten!</h1>
                </div>
                <div style='padding: 30px; line-height: 1.6;'>
                    <h3 style='color: #4b2c61;'>Kedves {user.Name}!</h3>
                    <p>Örömmel értesítünk, hogy regisztrációd sikeres volt a <strong>Wild Cut Fodrászat</strong> online rendszerébe.</p>
                    <p>Mostantól bármikor egyszerűen és gyorsan foglalhatsz időpontot kedvenc fodrászodhoz, nyomon követheted korábbi látogatásaidat és kezelheted profilodat.</p>
                    
                    <div style='background-color: #f9f4fb; border-left: 4px solid #4b2c61; padding: 15px; margin: 20px 0;'>
                        <p style='margin: 0;'><strong>Regisztrált e-mail cím:</strong> {user.Email}</p>
                    </div>

                    <p style='text-align: center; margin-top: 30px;'>
                        <a href='{verifyUrl}' style='display: inline-block; padding: 14px 25px; background-color: #4b2c61; color: #ffffff; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 16px; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>Belépés és Időpontfoglalás</a>
                    </p>
                    
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 30px 0;' />
                    <p style='font-size: 13px; color: #777; text-align: center;'>
                        Ha bármilyen kérdésed van, keress minket bizalommal!<br>
                        Wild Cut Fodrászat - 3000 Hatvan, Kazinczy u. 3.
                    </p>
                </div>
            </div>";
            await _emailService.SendEmailAsync(user.Email, subject, body);
            TempData["msg"] = "Kérlek erősítsd meg az email címed!";

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> MAAppointment()
        {
            var fodraszok = await _appointService.GetAllHairdressers();
            var szolgaltatasok = await _appointService.GetAllServices();

            if (!fodraszok.Success)
            {
                TempData["error_msg"] = fodraszok.Error;
                return RedirectToAction("Index", "Home");
            }

            if (!szolgaltatasok.Success)
            {
                TempData["error_msg"] = szolgaltatasok.Error;
                return RedirectToAction("Index", "Home");
            }

            var model = new AppointmentDTO
            {
                Appointment = new MAAppointmentViewModel { UserId = _currentUserService.UserId ?? 0 },
                Hairdressers = fodraszok.Data,
                Services = szolgaltatasok.Data!.Where(s => s.isActive).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment(AppointmentDTO model)
        {
            // 1) védekezés nullok ellen
            if (model?.Appointment == null)
            {
                TempData["error_msg"] = "Hibás adatok érkeztek.";
                await PopulateListsInModel(model!);
                return RedirectToAction("MAAppointment",model);
            }

            // 2) Kötelező mezők ellenőrzése (StartTime a slot választásból jön)
            if (model.Appointment.HairdresserId <= 0 || model.Appointment.ServiceId <= 0 || model.Appointment.StartTime == default)
            {
                TempData["error_msg"] = "Válassz fodrászt, szolgáltatást és időpontot!";
                await PopulateListsInModel(model);
                return RedirectToAction("MAAppointment",model);
            }

            

            // 3) Service betöltése DB-ből (NE model.Services-ből)
            var service = await _appointService.GetServiceById(model.Appointment.ServiceId);

            if (!service.Success)
            {
                TempData["error_msg"] = "A választott szolgáltatás nem található.";
                await PopulateListsInModel(model);
                return RedirectToAction("MAAppointment",model);
            }

            var user = await _userService.GetUserById(model.Appointment.UserId);
            var hairdresser = await _userService.GetUserById(model.Appointment.HairdresserId);

            var appointment = new Appointment
            {
                UserId = model.Appointment.UserId,
                HairdresserId = model.Appointment.HairdresserId,
                StartTime = model.Appointment.StartTime,
                EndTime = model.Appointment.StartTime.AddMinutes(service.Data!.DurationInMinute),
                ServiceId = service.Data.ServiceId,
                AppointmentStatus = AppointmentStatus.Booked,
                Notes = model.Appointment.Notes ?? null
            };

            var idoKulonbseg = appointment.StartTime - DateTime.Now;

            if (idoKulonbseg.TotalDays <= 2.5)
            {
                appointment.IsReminderSent = true;
            }
            else
            {
                appointment.IsReminderSent = false;
            }

            //-------------------------------------

            if (user.Success && hairdresser.Success)
            {
                user.Data!.Appointments.Add(appointment);
                hairdresser.Data!.HairdresserAppointments.Add(appointment);
            } 

            var result = await _appointService.CreateAppointment(appointment);
            if (!result.Success)
            {
                TempData["error_msg"] = result.Error;
                await PopulateListsInModel(model);
                return View("MAAppointment", model);
            }

            // --- Sikeres foglalás email ---
            string emailTargy = "Sikeres időpontfoglalás - Wild Cut Fodrászat";
            string emailUzenet = $@"
                <h3>Kedves {user.Data!.Name}!</h3>
                <p>Sikeresen rögzítettük az időpontodat!</p>
                <p><strong>Időpont:</strong> {appointment.StartTime.ToString("yyyy. MM. dd. HH:mm")}</p>
                <p><strong>Szolgáltatás:</strong> {service.Data!.Name}</p>
                <br/>
                <p>Várunk szeretettel!</p>";

            await _emailService.SendEmailAsync(user.Data.Email, emailTargy, emailUzenet);

            _logger.Log("INFO",$"UserId={model.Appointment.UserId} Appointment created (ServiceId={model.Appointment.ServiceId}, Time={model.Appointment.StartTime})");
            TempData["msg"] = "Sikeres időpontfoglalás";
            return RedirectToAction("Index", "Home");
        }

        private async Task PopulateListsInModel(AppointmentDTO model)
        {
            var hairdressers = await _appointService.GetAllHairdressers();
            model.Hairdressers = hairdressers.Data;
            var services = await _appointService.GetAllServices();
            model.Services = services.Data;
        }

        [HttpGet("/Account/Verify/{verificationToken}")]
        public async Task<IActionResult> Verify(string verificationToken)
        {
            var result = await _authService.VerifyByToken(verificationToken);
            if (!result.Success)
            {
                TempData["error_msg"] = result.Error;
                return RedirectToAction("Index", "Home");
            }

            TempData["msg"] = result.Data;
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int hairdresserId, string date, int serviceId)
        {
            var szolgaltatas = await _appointService.GetServiceById(serviceId);
            if (!szolgaltatas.Success)
                return Json(szolgaltatas.Error);

            var datum = DateOnly.Parse(date);

            var result = await _appointService.GetAvailableSlots(hairdresserId, datum, szolgaltatas.Data!.DurationInMinute);
            if (!result.Success)
                return Json(result.Error);

            return Json(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookedDays(int hairdresserId, string start, string end)
        {
            var kezdet = DateOnly.Parse(start);
            var vege = DateOnly.Parse(end);
            var result = await _appointService.GetBookedDays(hairdresserId, kezdet, vege);
            return Json(result.Data);
        }

        public async Task<bool> VerifyRecaptcha(string token)
        {
            var secret = "6LcZrYUsAAAAAKmqf7smog4u8Uw_M7b65sA90RDK";

            using var client = new HttpClient();

            var response = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}",
                null);

            var json = await response.Content.ReadAsStringAsync();

            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(json)!;

            return result.success == "true" && result.score >= 0.5;
        }
    }
}