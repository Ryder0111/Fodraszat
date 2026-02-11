using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models.Entities
{
    public class Service
    {
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Add meg a szolgáltatás nevét!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Add meg a szolgáltatás időtartamát!")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Add meg a szolgáltatás árát!")]
        public int Price { get; set; }

    }
}
