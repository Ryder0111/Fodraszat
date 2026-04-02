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

        public UserController(IAppointmentService appointmentService, ICurrentUserService currentUserService, IUserService userService)
        {
            _AppointmentService = appointmentService;
            _CurrentUserService = currentUserService;
            _UserService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var idopontok = await _AppointmentService.GetUserAppointments(_CurrentUserService.UserId);
            if (!idopontok.Success)
            {
                TempData["error_msg"] = idopontok.Error;
                return View();
            }
            
            return View(idopontok.Data);
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
                TempData["error_msg"] = "Nincs ilyen felhasználó!"; // Ez jött fel nálad pirossal
                return View(model);
            }

            var user = response.Data;

            // Csak akkor írjuk át, ha nem üres a mező
            if (!string.IsNullOrWhiteSpace(model.Name)) user!.Name = model.Name;
            if (!string.IsNullOrWhiteSpace(model.Email)) user!.Email = model.Email;
            user!.Sex = model.Sex;

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                // Készítünk egy egyedi fájlnevet (pl: a4b1-9c... + .jpg)
                var fileExtension = Path.GetExtension(model.ProfileImage.FileName);
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                // Meghatározzuk, hova mentse. A wwwroot mappán belül egy images/profiles mappába.
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");

                // Ha nem létezik még ez a mappa, létrehozzuk
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // A teljes mentési útvonal
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Kimásoljuk a szerverre a képet
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(fileStream);
                }

                // És végül beállítjuk a felhasználónál az elérési utat, amit a HTML-ben használni fogunk
                user.ProfileImageUrl = "/images/profiles/" + uniqueFileName;
            }

            var updateResult = await _UserService.UpdateUser(user);

            TempData["msg"] = "A profilod sikeresen frissült!";
            return RedirectToAction("EditProfile");
        } 

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var idopont = await _AppointmentService.CancelAppointment(id);
            if (!idopont.Success)
            {
                TempData["error_msg"] = idopont.Error;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["msg"] = "Sikeres törlés";
                return RedirectToAction("Index");
            }
        }
    }
}
