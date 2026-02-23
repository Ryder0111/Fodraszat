namespace FodraszatIdopont.Models.Enums
{
    [Flags]
    public enum UserRole
    {
        None = 0,
        User = 1,
        Hairdresser = 2,
        Admin = 4
    }
}
