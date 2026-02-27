using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models.ViewModels
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Az email cím megadása kötelező!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "A jelszó megadása kötelező!")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }
    }
}
