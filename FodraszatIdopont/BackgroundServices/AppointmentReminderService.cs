using Azure.Core;
using FodraszatIdopont.Helpers;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services.Interface;
using Microsoft.Extensions.Configuration; // Erre szükség van az IConfiguration miatt!

namespace FodraszatIdopont.BackgroundServices
{
    public class AppointmentReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LoggerHelper _logger;

        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public AppointmentReminderService(IServiceProvider serviceProvider, LoggerHelper logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;

            _baseUrl = _configuration["BaseUrl"] ?? "https://localhost:7294"; //biztonsági okból ha nem lenne benne az appsettingsben 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var appointmentRepo = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                        var celDatum = DateTime.Today.AddDays(2);

                        var erintettFoglalasok = await appointmentRepo.GetAppointmentsForReminderAsync(celDatum);

                        foreach (var app in erintettFoglalasok)
                        {
                            try
                            {
                                string subject = "Közeledő időpontod a Wild Cut Fodrászatnál!";

                                string body = $@"
                                    <div style='font-family: Arial, sans-serif; color: #333;'>
                                        <h3 style='color: #4b2c61;'>Kedves {app.User!.Name}!</h3>
                                        <p>Szeretnénk emlékeztetni, hogy 3 nap múlva időpontod van nálunk!</p>
                                        <p><strong>Időpont:</strong> {app.StartTime.ToString("yyyy. MM. dd. HH:mm")}</p>
                                        <p>Ha esetleg mégsem tudsz eljönni, kérjük mondd le az időpontot, hogy más átvehesse!</p>
                                        <br/>
                                        <a href='{_baseUrl}/User' style='display: inline-block; padding: 12px 20px; background-color: #4b2c61; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>Időpontjaim megtekintése / Lemondás</a>
                                    </div>";

                                await emailService.SendEmailAsync(app.User.Email, subject, body);

                                app.IsReminderSent = true;

                                _logger.Log("INFO", $"Reminder sent (AppointmentId={app.AppointmentId})");

                                await appointmentRepo.Update(app);
                            }
                            catch (Exception ex)
                            {
                                _logger.Log("ERROR", $"Reminder failed (AppointmentId={app.AppointmentId}, Error={ex.Message})");
                            }
                        }
                    }

                    var holnapReggel = DateTime.Today.AddDays(1).AddHours(8);
                    var varakozasiIdo = holnapReggel - DateTime.Now;

                    if (varakozasiIdo.TotalMilliseconds <= 0)
                    {
                        varakozasiIdo = varakozasiIdo.Add(TimeSpan.FromDays(1));
                    }

                    await Task.Delay(varakozasiIdo, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.Log("ERROR", $"Reminder service crashed (Error={ex.Message})");
            }
        }
    }
}