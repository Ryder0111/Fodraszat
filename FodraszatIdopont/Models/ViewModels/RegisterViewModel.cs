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
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "A jelszó megadása kötelező!")]
        [StringLength(7,ErrorMessage = "A jelszó minimum 7 karakter lehet!")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "A jelszó újboli megadása kötelező!")]
        [Compare("Password", ErrorMessage = "A két jelszó nem egyezik!")]
        public string ConfirmPassword { get; set; } = null!;

        public Gender Sex { get; set; }
    }
}
