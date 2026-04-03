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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FodraszatIdopont.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAppointmentService _appointService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;
        public AccountController(IAuthService authService, IAppointmentService appointService, ICurrentUserService currentUserService, IUserService userService, IWebHostEnvironment env)
        {
            _authService = authService;
            _appointService = appointService;
            _currentUserService = currentUserService;
            _userService = userService;
            _env = env;
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
                WriteToLog($"! Sikertelen robot bejelentkezlsés {model.Email} fiókkal !");
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
                return View(model);
            }

            var user = result.Data;
            await SignInUserAsync(user!, model.RememberMe);
            WriteToLog($"{user!.UserId} - {user.Email} - bejelentkezés");

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
            WriteToLog($"{userId} - kijelentkezés");

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

            if (!ModelState.IsValid) return View(model: felhasznalo);
            User user = new User()
            {
                Name = felhasznalo.Name,
                Phone = felhasznalo.Phone,
                Email = felhasznalo.Email,
                PasswordHash = PasswordHelper.HashPassword(felhasznalo.Password),
                Sex = felhasznalo.Sex,
            };
            var result = await _authService.RegisterAsync(user, felhasznalo.Password);
            if (!result.Success)
            {
                TempData["error_msg"] = result.Error;
                return View(felhasznalo);
            }

            await SignInUserAsync(user, false);
            WriteToLog($"{user.UserId} - {user.Email} - regisztráció");

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

            if (service == null)
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

            if(user.Success && hairdresser.Success)
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

            WriteToLog($"{model.Appointment.UserId} - időpontgolalás");
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

        public async Task SignInUserAsync(User user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name,          user.Name),
                new Claim(ClaimTypes.Email,         user.Email),
                new Claim(ClaimTypes.Role,          user.Role.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,

                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null,
                IssuedUtc = rememberMe ? DateTimeOffset.UtcNow : null
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties
            );
        }

        private void WriteToLog(string message)
        {
            var rootPath = _env.ContentRootPath; //a projekt gyökere

            var logDirectory = Path.Combine(rootPath, "Log");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var filePath = Path.Combine(logDirectory, "Logs.txt");

            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";

            System.IO.File.AppendAllText(filePath, logEntry);
        }
    }
}