using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models.Entities
{
    public class Service
    {
        public int ServiceId { get; set; }

        public string Name { get; set; } = null!;

        public int DurationInMinute { get; set; }

        public int Price { get; set; }

        public List<Appointment> Appointments { get; set; } = new();

    }
}
