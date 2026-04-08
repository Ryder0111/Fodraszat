using FodraszatIdopont.Controllers;
using FodraszatIdopont.Helpers;
using FodraszatIdopont.Services.Interface;

namespace FodraszatIdopont.BackgroundServices
{
    public class AppointmentCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IWebHostEnvironment _env;

        public AppointmentCleanupService(IServiceScopeFactory scopeFactory, IWebHostEnvironment env)
        {
            _scopeFactory = scopeFactory;
            _env = env;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();

                    try
                    {
                        int count = await appointmentService.AutoCompletePastAppointmentsAsync();
                        if (count > 0)
                        {
                            LoggerHelper.WriteToLog($"Automata lezárás: {count} db időpont készre állítva.", _env.ContentRootPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.WriteToLog($"HIBA az automata lezárásnál: {ex.Message}", _env.ContentRootPath);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
            }
        }
    }
}
