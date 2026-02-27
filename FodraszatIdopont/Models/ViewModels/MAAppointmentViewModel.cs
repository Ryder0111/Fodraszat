using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Models.ViewModels
{
    public class MAAppointmentViewModel
    {
        public int UserId { get; set; }
        public int HairdresserId { get; set; }
        public int ServiceId { get; set; }
        public DateTime StartTime { get; set; }
        public string Comment { get; set; }
    }
}
