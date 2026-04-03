using FodraszatIdopont.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FodraszatIdopont.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone {  get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public Gender Sex { get; set; }

        public UserRole Role { get; set; }

        // Megmondjuk, hogy ez a lista az Appointment tábla "User" tulajdonságához (vendég) kapcsolódik
        [InverseProperty("User")]
        public List<Appointment> Appointments { get; set; } = new();

        // Megmondjuk, hogy ez a lista az Appointment tábla "Hairdresser" tulajdonságához (fodrász) kapcsolódik
        [InverseProperty("Hairdresser")]
        public List<Appointment> HairdresserAppointments { get; set; } = new();

        public string? ProfileImageUrl { get; set; }

    }
}
