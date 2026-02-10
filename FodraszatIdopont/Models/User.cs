namespace FodraszatIdopont.Models
{
    public class User
    {
        PasswordHelper passwordHelper;
        public int UserId {  get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Sex { get; set; }
        public User(int userId, string name, string email, string passwordHash, string sex)
        {
            UserId = userId;
            Name = name;
            Email = email;
            PasswordHash = passwordHelper.HashPassword(passwordHash);
            Sex = sex;
        }
    }
}
