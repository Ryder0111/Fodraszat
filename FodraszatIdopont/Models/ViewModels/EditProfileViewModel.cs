using FodraszatIdopont.Models.Enums;
using Microsoft.AspNetCore.Http; // Ez kell az IFormFile-hoz!
using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models.ViewModels
{
    public class EditProfileViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "A név megadása kötelező!")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Az email megadása kötelező!")]
        [EmailAddress(ErrorMessage = "Nem megfelelő az email formátuma!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Válaszd ki a nemed!")]
        public Gender Sex { get; set; }

        private string _phone = null!;

        [Required(ErrorMessage = "A telefonszám megadása kötelező!")]
        [Phone(ErrorMessage = "Nem megfelelő telefonszám formátum!")]
        [DisplayFormat(DataFormatString = "{0:+## (##) ###-####}")]
        public string Phone
        {
            get => _phone;
            set => _phone = value != null ? new string(value.Where(char.IsDigit).ToArray()) : null!;
        }

        // Új mezők a profilképhez:
        public IFormFile? ProfileImage { get; set; } // Ide jön be a feltöltött fájl
        public string? CurrentProfileImageUrl { get; set; } // Ezt jelenítjük meg, ha már van képe
    }
}