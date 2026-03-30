namespace FodraszatIdopont.Models.ViewModels
{
    public class AppointmentViewModel
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
        public string ServiceName { get; set; } = null!;
        public string HairdresserName { get; set; } = null!;

    }
}
