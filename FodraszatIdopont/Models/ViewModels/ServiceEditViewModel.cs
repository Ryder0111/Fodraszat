namespace FodraszatIdopont.Models.ViewModels
{
    public class ServiceEditViewModel : ServiceViewModel
    {
        public int ServiceId { get; set; }

        public bool isActive { get; set; }

        public int AppointmentCount { get; set; }
    }
}