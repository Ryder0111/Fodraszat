using FodraszatIdopont.Controllers;
using FodraszatIdopont.Data;
using FodraszatIdopont.Helpers;
using FodraszatIdopont.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FodraszatIdopont.BackgroundServices
{
    public class AppointmentCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly LoggerHelper _logger;

        public AppointmentCleanupService(IServiceScopeFactory scopeFactory, LoggerHelper logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
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
                                _logger.Log("INFO", $"Auto complete: {count} appointments updated");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Log("ERROR", $"Auto complete failed (Error={ex.Message})");
                        }
                    }

                    await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
                }
            }
            catch (OperationCanceledException) when(stoppingToken.IsCancellationRequested)
                {
                    
                }
            catch (Exception ex)
            {
                _logger.Log("ERROR", $"Cleanup service crashed (Error={ex.Message})");
            }
        }
    }
}
