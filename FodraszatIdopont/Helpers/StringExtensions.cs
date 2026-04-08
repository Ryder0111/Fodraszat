namespace FodraszatIdopont.Helpers
{
    public static class StringExtensions
    {
        public static string ToFormattedPhone(this string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return "Nincs megadva";
            var digits = new string(phone.Where(char.IsDigit).ToArray());
            return digits.Length == 11
                ? $"+{digits[..2]} ({digits[2..4]}) {digits[4..7]}-{digits[7..]}"
                : phone;
        }
    }
}
