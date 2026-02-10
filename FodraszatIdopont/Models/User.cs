using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }
        public string Sex { get; set; }
    }
}
