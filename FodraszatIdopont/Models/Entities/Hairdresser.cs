using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models.Entities
{
    public class Hairdresser
    {
        public int HairdresserId { get; set; }

        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public List<Appointment> Appointments { get; set; } = new();

    }
}
