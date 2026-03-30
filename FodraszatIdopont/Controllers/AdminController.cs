using FodraszatIdopont.Models;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.ViewModels;
using FodraszatIdopont.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FodraszatIdopont.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAdminService _adminService;
        private readonly IAppointmentService _appointmentService;
        public AdminController(IUserService userService, IAdminService adminService, IAppointmentService appointmentService)
        {
            _userService = userService;
            _adminService = adminService;
            _appointmentService = appointmentService;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var response = await _userService.GetAllUsers();
            if(!response.Success)
            {
                TempData["error_msg"] = response.Error;
                return RedirectToAction("Index");
            }
            return View(response.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _userService.GetUserById(id);
            if (!response.Success)
            {
                TempData["error_msg"] = response.Error;
                return RedirectToAction("Index");
            }
            return View(response.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string userEmail, Models.Enums.UserRole newRole)
        {
            if(newRole == Models.Enums.UserRole.Hairdresser)
            {
                var response = await _adminService.PromoteToHairdresser(userEmail);
                if (!response.Success)
                {
                    TempData["error_msg"] = response.Error;
                    return RedirectToAction("Users");
                }
                TempData["msg"] = $"{response.Data!.Name} mostmár fodrász!";
                return RedirectToAction("Details", new { id = response.Data.UserId });
            }
            else
            {
                var response = await _adminService.RemoveHairdresserRole(userEmail);
                if (!response.Success)
                {
                    TempData["error_msg"] = response.Error;
                    return RedirectToAction("Users");
                }
                TempData["msg"] = $"{response.Data!.Name} mostmár csak vendég!";
                return RedirectToAction("Details", new { id = response.Data.UserId });
            }
        }

        public async Task<IActionResult> Services()
        {
            var response = await _appointmentService.GetAllServices();
            if (!response.Success)
            {
                TempData["error_msg"] = response.Error;
                return RedirectToAction("Index");
            }
            var dto = new ServiceDTO
            {
                Services = response.Data!,
                NewService = new()
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService([Bind(Prefix = "NewService")]/*Hogy megmaradjanak a formban az értkek*/ ServiceViewModel model)
        {
            if(!ModelState.IsValid)
            {
                TempData["error_msg"] = "Az adatok megadása sikertelen!";
                return View("Services", await PopulateListsInModel());
            }

            Service service = new Service
            {
                Name = model.Name,
                DurationInMinute = model.DurationInMinute,
                Price = model.Price,
                isActive = true
            };

            var response = await _appointmentService.CreateService(service);
            if (!response.Success)
            {
                TempData["error_msg"] = response.Error;
                return View("Services", await PopulateListsInModel());
            }

            TempData["msg"] = $"{response.Data!.Name} szolgáltatás létrehozva!";
            return View("Services", await PopulateListsInModel());
        }
        private async Task<ServiceDTO> PopulateListsInModel()
        {
            ServiceDTO model = new();
            var services = await _appointmentService.GetAllServices();
            model.Services = services.Data!;
            model.NewService = new();
            return model;
        }

        public async Task<IActionResult> serviceDetails(int id)
        {
            var respone = await _appointmentService.GetServiceById(id);
            if (!respone.Success)
            {
                TempData["error_msg"] = respone.Error;
                return View("Services", await PopulateListsInModel());
            }

            var service = respone.Data!;

            var viewModel = new ServiceEditViewModel
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                DurationInMinute = service.DurationInMinute,
                Price = service.Price,
                isActive = service.isActive,
                AppointmentCount = service.Appointments.Count
            };
            return View(viewModel);
        }
    }
}
