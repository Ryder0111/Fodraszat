using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models.Entities
{
    public class Hairdresser
    {
        public int HairdresserId { get; set; }

        [Required(ErrorMessage = "A fodrász nevének megadása kötelező!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A telefonszám megadása kötelező!")]
        public string Phone { get; set; }

    }
}
