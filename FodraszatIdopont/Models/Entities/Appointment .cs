using FodraszatIdopont.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int HairdresserId { get; set; }
        public User? Hairdresser { get; set; }

        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public AppointmentStatus AppointmentStatus { get; set; }



    }
}
