using FodraszatIdopont.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int HairdresserId { get; set; }
        public User Hairdresser { get; set; } = null!;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public AppointmentStatus AppointmentStatus { get; set; }
        public string Comment { get; set; }



    }
}
