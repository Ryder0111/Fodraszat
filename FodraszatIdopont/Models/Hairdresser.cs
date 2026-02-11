using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models
{
    public class Hairdresser
    {
        public string HairdresserId { get; set; }
        [Required(ErrorMessage = "A fodrász nevének megadása kötelező!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "A telefonszám megadása kötelező!")]
        public string Phone { get; set; }
        public Hairdresser(string hairdresserId, string name, string phone)
        {
            HairdresserId = hairdresserId;
            Name = name;
            Phone = phone;
        }
    }
}
