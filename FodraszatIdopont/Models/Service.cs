using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models
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
        public Service(int serviceId, string name, int duration, int price)
        {
            ServiceId = serviceId;
            Name = name;
            Duration = duration;
            Price = price;
        }
    }
}
