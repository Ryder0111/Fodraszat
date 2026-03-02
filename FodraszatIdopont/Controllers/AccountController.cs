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
        public AccountController(IAuthService authService, IAppointmentService appointService, ICurrentUserService currentUserService)
        {
            _authService = authService;
            _appointService = appointService;
            _currentUserService = currentUserService;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //CSRF elleni védelem; CSRF-Cross-site request forgery
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error_msg"] = "Hiba történt! Prbáld újra!";
                return View(model);
            }

            var result = await _authService.AuthenticateAsync(model.Email, model.Password);

            if (!result.Success)
            {
                TempData["error_msg"] = result.Error ?? "Hibás email vagy jelszó";
                return View(model);
            }

            var user = result.Data;

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name,          user.Name),
        new Claim(ClaimTypes.Email,         user.Email),
        //new Claim(ClaimTypes.Role,          user.Role.ToString()),
    };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,

                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null,
                IssuedUtc = model.RememberMe ? DateTimeOffset.UtcNow : null
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties
            );

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("FodraszatAuth");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(RegisterViewModel felhasznalo)
        {
            if (!ModelState.IsValid) return View(model: felhasznalo);
            User user = new User()
            {
                Name = felhasznalo.Name,
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


            LoginViewModel bejelent = new LoginViewModel()
            {
                Email = felhasznalo.Email,
                Password = felhasznalo.Password
            };
            return await Login(bejelent);
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
                Appointment = new MAAppointmentViewModel { UserId = _currentUserService.UserId ?? 0},
                Hairdressers = fodraszok.Data,
                Services = szolgaltatasok.Data
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment(AppointmentDTO model)
        {
            if (!ModelState.IsValid)
            {
                var fodraszok = await _appointService.GetAllHairdressers();
                var szolgaltatasok = await _appointService.GetAllServices();
                model.Hairdressers = fodraszok.Data;
                model.Services = szolgaltatasok.Data;
                return View("MAAppointment", model);
            }

            var service = model.Services.FirstOrDefault(s => s.ServiceId == model.Appointment.ServiceId);
            if (service == null)
            {
                TempData["error_msg"] = "Érvénytelen szolgáltatás!";
                return View("MAAppointment", model);
            }

            var appointment = new Appointment
            {
                UserId = model.Appointment.UserId,
                HairdresserId = model.Appointment.HairdresserId,
                StartTime = model.Appointment.StartTime,
                EndTime = model.Appointment.StartTime.AddMinutes(service.DurationInMinute),
                ServiceId = service.ServiceId,
                AppointmentStatus = AppointmentStatus.Booked
            };

            var result = await _appointService.CreateAppointment(appointment);
            if (!result.Success)
            {
                TempData["error_msg"] = result.Error;
                return View("MAAppointment", model);
            }

            TempData["msg"] = "Sikeres időpontfoglalás";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int hairdresserId, string date, int serviceId)
        {
            var szolgaltatas = await _appointService.GetServiceById(serviceId);
            var datum = DateOnly.Parse(date);
            var result = await _appointService.GetAvailableSlots(hairdresserId, datum, szolgaltatas.Data.DurationInMinute);
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
    }
}