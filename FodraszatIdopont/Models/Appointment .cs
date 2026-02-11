using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int HairdresserId { get; set; }
        [Required]
        public int ServiceId { get; set; }
        public DateTime DateTime { get; set; }
        public Appointment(int appointmentId, int userId, int hairdresserId, int serviceId, DateTime dateTime)
        {
            AppointmentId = appointmentId;
            UserId = userId;
            HairdresserId = hairdresserId;
            ServiceId = serviceId;
            DateTime = dateTime;
        }
    }
}
