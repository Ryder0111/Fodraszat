using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.Enums;
using FodraszatIdopont.Models.ViewModels;
using FodraszatIdopont.Services;
using FodraszatIdopont.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FodraszatIdopont.Controllers
{

    [Authorize]
    public class UserController : Controller
    {

        public readonly IAppointmentService _AppointmentService;
        public readonly ICurrentUserService _CurrentUserService;
        private readonly IUserService _UserService;
        private readonly IAuthService _AutService;
        private readonly LoggerHelper _logger;

        public UserController(IAppointmentService appointmentService, ICurrentUserService currentUserService, IUserService userService, IAuthService autService, LoggerHelper logger)
        {
            _AppointmentService = appointmentService;
            _CurrentUserService = currentUserService;
            _UserService = userService;
            _AutService = autService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var idopontok = await _AppointmentService.GetUserAppointments(_CurrentUserService.UserId);
            if (!idopontok.Success)
            {
                TempData["error_msg"] = idopontok.Error;
                return View();
            }
            
            return View("Indexu",idopontok.Data);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _CurrentUserService.GetCurrentUserAsync();

            var model = new EditProfileViewModel
            {
                UserId = user!.UserId,
                Name = user.Name,
                Email = user.Email,
                Sex = user.Sex,
                Phone = user.Phone,
                CurrentProfileImageUrl = user.ProfileImageUrl
            };

            return View(model); // Így az űrlap már kitöltve jelenik meg!
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            // Ha Admin, töröljük a hibaüzeneteket (pl. az érvénytelen email formátumot)
            if (_CurrentUserService.Roles.HasFlag(UserRole.Admin))
            {
                ModelState.Clear();
            }

            if (!ModelState.IsValid) return View(model);

            var response = await _UserService.GetUserById(model.UserId);
            if (!response.Success)
            {
                TempData["error_msg"] = "Nincs ilyen felhasználó!";
                _logger.Log("ERROR", $"UserId={model.UserId} Profile update failed");
                return View(model);
            }

            var user = response.Data;

            // Csak akkor írjuk át, ha nem üres a mező
            if (!string.IsNullOrWhiteSpace(model.Name)) user!.Name = model.Name;
            if (!string.IsNullOrWhiteSpace(model.Email)) user!.Email = model.Email;
            if (!string.IsNullOrWhiteSpace(model.Phone)) user!.Phone = model.Phone;
            user!.Sex = model.Sex;

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                // 1. RÉGI KÉP TÖRLÉSE (ÚJ RÉSZ)
                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    // Megkeressük a régi fájl fizikai útvonalát
                    // A TrimStart('/') azért kell, hogy a "/images/..."-ból "images/..." legyen a Path.Combine-hoz
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfileImageUrl.TrimStart('/'));

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // 2. ÚJ KÉP MENTÉSE (Ami már megvolt nálatok)
                var fileExtension = Path.GetExtension(model.ProfileImage.FileName);
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(fileStream);
                }

                user.ProfileImageUrl = "/images/profiles/" + uniqueFileName;
            }

            await _AutService.SignInUserAsync(user, true); //Átírja a sütikben is a nevét

            var updateResult = await _UserService.UpdateUser(user);

            TempData["msg"] = "A profilod sikeresen frissült!";
            _logger.Log("INFO", $"UserId={user.UserId} Profile updated");
            return RedirectToAction("EditProfile");
        } 

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var idopont = await _AppointmentService.CancelAppointment(id);
            if (!idopont.Success)
            {
                TempData["error_msg"] = idopont.Error;
                _logger.Log("ERROR", $"UserId={_CurrentUserService.UserId} Cancel failed (Id={id}, Error={idopont.Error})");
                return RedirectToAction("Index");
            }
            else
            {
                TempData["msg"] = "Sikeres törlés";
                _logger.Log("INFO", $"UserId={_CurrentUserService.UserId} Appointment cancelled (Id={id})");
                return RedirectToAction("Index");
            }
        }
    }
}
