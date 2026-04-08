namespace FodraszatIdopont.Helpers
{
    public static class LoggerHelper
    {
        public static void WriteToLog(string message, string rootPath)
        {
            var logDirectory = Path.Combine(rootPath, "Log");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var filePath = Path.Combine(logDirectory, "Logs.txt");
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";

            System.IO.File.AppendAllText(filePath, logEntry);
        }
    }
}