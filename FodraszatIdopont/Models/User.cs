using System.ComponentModel.DataAnnotations;

namespace FodraszatIdopont.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "A név megadása kötelező!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Az email megadása kötelező!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A jelszó megadása kötelező!")]
        public string PasswordHash { get; set; }
        public string Sex { get; set; }

        public User(int userId, string name, string email, string password, string sex)
        {
            UserId = userId;
            Name = name;
            Email = email;
            PasswordHash =  PasswordHelper.HashPassword(password);
            Sex = sex;
        }
    }
}
