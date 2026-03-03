using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.Enums;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services.Interface;
using System;

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
                return Results<Appointment>.Fail("Válassz fodrász!");

            if (appointment.UserId == appointment.HairdresserId)
                return Results<Appointment>.Fail("Nem lehetsz saját magad vendége!😉");


            var szolgaltatas = await _Servicerepo.GetById(appointment.ServiceId);
            if (szolgaltatas == null)
            {
                return Results<Appointment>.Fail("Válassz szolgáltatás!");
            }


            if (await _Appointmentrepo.ExistsInTimeRange(appointment.HairdresserId,appointment.StartTime,appointment.EndTime))
            {
                return Results<Appointment>.Fail($"Ez az időpont({appointment.StartTime.ToString("MM. dd. HH:mm")}) már foglalt");
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

        public async Task<Results<List<DateTime>>> GetAvailableSlots(int hairdresserId, DateOnly date, int serviceDurationInMinutes)
        {
            if (date.DayOfWeek == DayOfWeek.Sunday)
                return Results<List<DateTime>>.Fail("Vasárnap zárva vagyunk.");

            var appointments = await _Appointmentrepo.GetAppointmentsByDateAndHairdresser(hairdresserId, date);
            var ordered = appointments.Where(a => a.AppointmentStatus != AppointmentStatus.Cancelled)
                                      .OrderBy(a => a.StartTime)
                                      .ToList();

            var slots = new List<DateTime>();
            var current = date.ToDateTime(new TimeOnly(10, 0));
            var closing = date.ToDateTime(new TimeOnly(18, 0));

            while (current + TimeSpan.FromMinutes(serviceDurationInMinutes) <= closing)
            {
                bool conflict = false;
                var proposedEnd = current + TimeSpan.FromMinutes(serviceDurationInMinutes);

                foreach (var app in ordered)
                {
                    if (current < app.EndTime && proposedEnd > app.StartTime)
                    {
                        conflict = true;
                        current = app.EndTime;  // ugrás a következő szabadra
                        break;
                    }
                }

                if (!conflict)
                {
                    slots.Add(current);
                    current += TimeSpan.FromMinutes(15);  // finomabb lépés, ha kell – vagy 60 perc
                }
            }

            return slots.Any()
                ? Results<List<DateTime>>.Ok(slots)
                : Results<List<DateTime>>.Fail("Nincs szabad időpont.");
        }

        public async Task<Results<List<DateOnly>>> GetBookedDays(int hairdresserId, DateOnly startDate, DateOnly endDate)
        {
            var bookedDays = new List<DateOnly>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                if (currentDate.DayOfWeek >= DayOfWeek.Monday && currentDate.DayOfWeek <= DayOfWeek.Saturday)
                {
                    var appointments = await _Appointmentrepo.GetAppointmentsByDateAndHairdresser(hairdresserId, currentDate);

                    if (appointments == null || !appointments.Any())
                    {
                        currentDate = currentDate.AddDays(1);
                        continue;
                    }

                    var ordered = appointments
                        .Where(a => a.AppointmentStatus != AppointmentStatus.Cancelled)
                        .OrderBy(a => a.StartTime)
                        .ToList();

                    if (IsFullyBooked(ordered, 10, 18))
                    {
                        bookedDays.Add(currentDate);
                    }
                }

                currentDate = currentDate.AddDays(1);
            }

            return Results<List<DateOnly>>.Ok(bookedDays);
        }

        // Segédfüggvény: ellenőrzi, hogy az adott nap teljesen foglalt-e
        private bool IsFullyBooked(List<Appointment> apps, int startHour, int endHour)
        {
            if (apps == null || !apps.Any()) return false;

            var start = new TimeOnly(startHour, 0);
            var end = new TimeOnly(endHour, 0);
            var current = start;

            foreach (var app in apps)
            {
                var appStart = TimeOnly.FromDateTime(app.StartTime);
                var appEnd = TimeOnly.FromDateTime(app.EndTime);

                if (current < appStart && appStart - current >= TimeSpan.FromMinutes(45))
                    return false;

                current = appEnd > current ? appEnd : current;
            }

            return end - current < TimeSpan.FromMinutes(45);
        }

        public async Task<Results<Service>> GetServiceById(int serviceId)
        {
            var szolgaltatas = await _Servicerepo.GetById(serviceId);

            if (szolgaltatas == null)
                return Results<Service>.Fail("Nincs ilyen szolgáltatás!");

            return Results<Service>.Ok(szolgaltatas);
        }
    }
}
