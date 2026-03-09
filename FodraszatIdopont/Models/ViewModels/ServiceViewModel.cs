using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models.ViewModels
{
    public class ServiceViewModel
    {
        [Required(ErrorMessage = "A név megadása kötelező!")]
        [StringLength(50, MinimumLength = 4,ErrorMessage = "Legalább 4 karakter és maximum 50 hosszú legyen!")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "A időtartam megadása kötelező!")]
        [Range(10,180, ErrorMessage = "A hossz minimum 10, maximum 180 perc lehet!")]
        public int DurationInMinute { get; set; }

        [Required(ErrorMessage = "Az ár megadása kötelező!")]
        [Range(10, 30000, ErrorMessage = "Az ár 10Ft és 30.000Ft között kell lennie")]
        public int Price { get; set; }
    }
}
