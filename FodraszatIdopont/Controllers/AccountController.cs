using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
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
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken] //CSRF elleni védelem; CSRF-Cross-site request forgery
        //public async Task<IActionResult> Login(LoginViewModel felhasznalo)
        //{
        //    var user = await _authService.AuthenticateAsync(felhasznalo.Email, felhasznalo.Password);

        //    if (!user.Success)
        //    {
        //        TempData["error_msg"] = user.Error;
        //        return View();
        //    }
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, user.Data.UserId.ToString()),
        //        new Claim(ClaimTypes.Name, user.Data.Name),
        //        new Claim(ClaimTypes.Email, user.Data.Email),
        //        new Claim(ClaimTypes.Role, user.Data.Role.ToString())
        //    };
        //    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        //    await HttpContext.SignInAsync(
        //        CookieAuthenticationDefaults.AuthenticationScheme,
        //        claimsPrincipal
        //    );
        //    return RedirectToAction("Index", "Home");
        //}

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
                IssuedUtc  = model.RememberMe ? DateTimeOffset.UtcNow : null    
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
        public async Task<IActionResult> Registration(RegisterViewModel felhasznalo)
        {
            if(!ModelState.IsValid) return View(model: felhasznalo);
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
        public IActionResult MAAppointment()
        {
            return View();
        }
    }
}