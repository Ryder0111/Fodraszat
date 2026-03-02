using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.Enums;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services.Interface;

namespace FodraszatIdopont.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _Appointmentrepo;
        private readonly IServiceRepository _Servicerepo;
        private readonly IUserRepository _Userrepo;
        private readonly ICurrentUserService _CurrentUser;

        public AppointmentService(IAppointmentRepository repo1,IServiceRepository repo2, IUserRepository repo3, ICurrentUserService currentUser)
        {
            _Appointmentrepo = repo1;
            _Servicerepo = repo2;
            _Userrepo = repo3;
            _CurrentUser = currentUser;
        }

        public async Task<Results<List<User>>> GetAllHairdressers()
        {
            var fodraszok = await _Userrepo.GetAllHairdresser();
            if (fodraszok == null)
            {
                return Results<List<User>>.Fail("Még nincsenek fodrászok!");
            }
            return Results<List<User>>.Ok(fodraszok.ToList());
        }
        public async Task<Results<List<Service>>> GetAllServices()
        {
            var szolgaltatasok = await _Servicerepo.GetAll();
            if (szolgaltatasok == null)
            {
                return Results<List<Service>>.Fail("Még nincsenek szolgáltatások!");
            }

            return Results<List<Service>>.Ok(szolgaltatasok.ToList());
        }

        public async Task<Results<Appointment>> CancelAppointment(Appointment appointment)
        {
            var idopont = await _Appointmentrepo.GetById(appointment.AppointmentId);
            if (idopont == null)
            {
                return Results<Appointment>.Fail("Nincs ilyen időpontfoglalás!");

            }

            if (!_CurrentUser.Roles.HasFlag(UserRole.Admin))
            {
                if (_CurrentUser.UserId != idopont.UserId)
                {
                    if (_CurrentUser.UserId != idopont.HairdresserId)
                    {
                        return Results<Appointment>.Fail("Nincs jogod törölni az időpontot");
                    }
                }
            }

            if (idopont.AppointmentStatus == AppointmentStatus.Cancelled)
            {
                return Results<Appointment>.Fail("Ez az időpontfoglalás már le van mondva");
            }

            if (DateTime.Now.AddDays(1) > idopont.StartTime)
            {
                return Results<Appointment>.Fail("Ezt az időpontot már nem lehet lemondani.");
            }

            idopont.AppointmentStatus = AppointmentStatus.Cancelled;
            await _Appointmentrepo.Update(idopont);
            return Results<Appointment>.Ok(idopont);

        }

        public async Task<Results<Appointment>> CreateAppointment(Appointment appointment)
        {
            var hairdresser = await _Userrepo.GetById(appointment.HairdresserId);
            if (hairdresser == null || !hairdresser.Role.HasFlag(UserRole.Hairdresser))
                return Results<Appointment>.Fail("Érvénytelen fodrász.");

            if (appointment.UserId == appointment.HairdresserId)
                return Results<Appointment>.Fail("A fodrász nem lehet saját maga vendége.");


            var szolgaltatas = await _Servicerepo.GetById(appointment.ServiceId);
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

        public async Task<Results<List<Appointment>>> GetHairdresseSchedule(User hairdresser)
        {
            if (hairdresser == null)
                return Results<List<Appointment>>.Fail("Nincs ilyen fodrász");

            var fodrasz = await _Userrepo.GetById(hairdresser.UserId);

            if (fodrasz == null || !fodrasz.Role.HasFlag(UserRole.Hairdresser))
                return Results<List<Appointment>>.Fail("Nincs ilyen fodrász");

            else
            {
                var idopontok = await _Appointmentrepo.GetAppointmentsByDateAndHairdresser(fodrasz.UserId, DateOnly.FromDateTime(DateTime.Now));
                return Results<List<Appointment>>.Ok(idopontok);
            }
        }

        public async Task<Results<List<Appointment>>> GetUserAppointments(User user)
        {
            if (user == null)
                return Results<List<Appointment>>.Fail("Nincs ilyen felhasználó");

            var dbUser = await _Userrepo.GetById(user.UserId);

            if (dbUser == null)
                return Results<List<Appointment>>.Fail("Nincs ilyen felhasználó");

            return Results<List<Appointment>>.Ok(await _Appointmentrepo.GetFutureAppointmentsByUser(dbUser.UserId));
        }
    }
}
