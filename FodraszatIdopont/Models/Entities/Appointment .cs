using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int HairdresserId { get; set; }
        public Hairdresser Hairdresser { get; set; }

        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public DateTime DateTime { get; set; }

    }
}
