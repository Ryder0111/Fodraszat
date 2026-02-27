using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.ViewModels;

namespace FodraszatIdopont.Models
{
    public class AppointmentDTO
    {
        public MAAppointmentViewModel Appointment {  get; set; }
        public List<Service> Services { get; set; }
        public List<User> Hairdressers { get; set; }
       
    }
}
