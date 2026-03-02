using System.ComponentModel.DataAnnotations;  

namespace FodraszatIdopont.Models.ViewModels
{
    public class MAAppointmentViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Válassz fodrászt!")]
        public int HairdresserId { get; set; }

        [Required(ErrorMessage = "Válassz szolgáltatást!")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Válassz időpontot!")]
        [DataType(DataType.DateTime)]  
        public DateTime StartTime { get; set; }

        [StringLength(200, ErrorMessage = "Maximum 200 karakter!")]
        public string? Notes { get; set; }
    }
}