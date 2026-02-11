using FodraszatIdopont.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public Gender Sex { get; set; }

        public UserRole Role { get; set; }

        public List<Appointment> Appointments { get; set; } = new();

    }
}
