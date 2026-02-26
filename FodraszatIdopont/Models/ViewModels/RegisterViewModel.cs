using FodraszatIdopont.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace FodraszatIdopont.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "A név megadása kötelező!")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Az email megadása kötelező!")]
        [EmailAddress(ErrorMessage ="Nem megfelelő az email formátuma!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "A jelszó megadása kötelező!")]
        [MinLength(7,ErrorMessage = "A jelszónak minimum 7 karakterből kell állnia!")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage ="Töltsd ki a mezőt!")]
        [Compare("Password", ErrorMessage = "A két jelszó nem egyezik!")]
        public string ConfirmPassword { get; set; } = null!;

        public Gender Sex { get; set; }
    }
}
