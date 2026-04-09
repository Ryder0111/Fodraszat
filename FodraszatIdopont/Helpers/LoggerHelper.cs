using Microsoft.Extensions.Hosting;

namespace FodraszatIdopont.Helpers
{
    public class LoggerHelper
    {
        private readonly string _logFilePath;

        public LoggerHelper(IHostEnvironment env)
        {
            var logDirectory = Path.Combine(env.ContentRootPath, "Log");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            _logFilePath = Path.Combine(logDirectory, "Logs.txt");
        }

        public void Log(string level, string message)
        {
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {level} {message}{Environment.NewLine}";
            File.AppendAllText(_logFilePath, logEntry);
        }
    }
}