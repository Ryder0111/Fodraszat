using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services.Interface;

namespace FodraszatIdopont.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _Appointmentrepo;
        private readonly IServiceRepository _Servicerepo;

        public AppointmentService(IAppointmentRepository repo1,IServiceRepository repo2)
        {
            _Appointmentrepo = repo1;
            _Servicerepo = repo2;
        }

        public async Task<Results<Appointment>> CancelAppointment(Appointment appointment)
        {
            var idopont = await _Appointmentrepo.GetById(appointment.AppointmentId);

            if(idopont == null)
            {
                return Results<Appointment>.Fail("Nincs ilyen időpontfoglalás!");
            }
            
            if(idopont.AppointmentStatus == Models.Enums.AppointmentStatus.Cancelled)
            {
                return Results<Appointment>.Fail("Ez az időpontfoglalás már le van mondva");
            }

            if (DateTime.Now.AddDays(1) > idopont.StartTime)
            {
                return Results<Appointment>.Fail("Ezt az időpontot már nem lehet lemondani.");
            }

            await _Appointmentrepo.Update(idopont);
            return Results<Appointment>.Ok(idopont);
        }

        public async Task<Results<Appointment>> CreateAppointment(Appointment appointment, int ServiceId)
        {
            var szolgaltatas = await _Servicerepo.GetServiceById(ServiceId);
            if (szolgaltatas == null)
            {
                return Results<Appointment>.Fail("Nem létezik ilyen szolgáltatás");
            }

            appointment.EndTime = appointment.StartTime + TimeSpan.FromMinutes(szolgaltatas.DurationInMinute);

            if (await _Appointmentrepo.ExistsInTimeRange(appointment.HairdresserId,appointment.StartTime,appointment.EndTime))
            {
                return Results<Appointment>.Fail($"{appointment.StartTime.ToString("MM-dd. HH:mm")} már foglalt");
            }
            await _Appointmentrepo.Create(appointment);
            return Results<Appointment>.Ok(appointment);
        }

        public Task<Results<List<Appointment>>> GetHairdresseSchedule(Hairdresser hairdresser)
        {
            throw new NotImplementedException();

        public Task<Results<List<Appointment>>> GetUserAppointments(User user)
        {
            throw new NotImplementedException();
        }
    }
}
