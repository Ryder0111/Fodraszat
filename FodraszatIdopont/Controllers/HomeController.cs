using Azure;
using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FodraszatIdopont.Controllers
{
    public class HomeController : Controller
    {
        private readonly LoggerHelper _logger;
        private readonly IAppointmentService _appointmentService;

        public HomeController(LoggerHelper logger,IAppointmentService appointmentService)
        {
            _logger = logger;
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _appointmentService.GetAllServices();

            if (!response.Success)
            {
                _logger.Log("ERROR", $"GetAllServices failed: {response.Error}");
                return View(new List<Service>());
            }

            var services = response.Data;
            return View(services);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
