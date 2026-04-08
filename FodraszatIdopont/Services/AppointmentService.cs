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

        public AppointmentService(IAppointmentRepository repo1, IServiceRepository repo2, IUserRepository repo3, ICurrentUserService currentUser)
        {
            _Appointmentrepo = repo1;
            _Servicerepo = repo2;
            _Userrepo = repo3;
            _CurrentUser = currentUser;
        }

        public async Task<Results<List<User>>> GetAllHairdressers()
        {
            var fodraszok = await _Userrepo.GetAllHairdresser();
            if (!fodraszok.Any())
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

        public async Task<Results<Appointment>> CompleteAppointment(int appointmentid)
        {
            var idopont = await _Appointmentrepo.GetById(appointmentid);
            if (idopont == null)
            {
                return Results<Appointment>.Fail("Nincs ilyen időpontfoglalás!");

            }

            if (!_CurrentUser.Roles.HasFlag(UserRole.Hairdresser))
            {
                if (_CurrentUser.UserId != idopont.UserId)
                {
                    if (_CurrentUser.UserId != idopont.HairdresserId)
                    {
                        return Results<Appointment>.Fail("Nincs jogod műveletet végrehajtani");
                    }
                }
            }

            if (idopont.AppointmentStatus == AppointmentStatus.Completed || idopont.AppointmentStatus == AppointmentStatus.Cancelled)
            {
                return Results<Appointment>.Fail("Ez már nem létezik");
            }

            idopont.AppointmentStatus = AppointmentStatus.Completed;
            await _Appointmentrepo.Update(idopont);
            return Results<Appointment>.Ok(idopont);
        }

        public async Task<Results<Appointment>> CancelAppointment(int apoointmentid)
        {
            var idopont = await _Appointmentrepo.GetById(apoointmentid);
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

            if (!_CurrentUser.Roles.HasFlag(FodraszatIdopont.Models.Enums.UserRole.Hairdresser))
            {
                if (DateTime.Now.AddDays(1) > idopont.StartTime)
                {
                    return Results<Appointment>.Fail("Ezt az időpontot már nem lehet lemondani.");
                }
            }

            idopont.AppointmentStatus = AppointmentStatus.Cancelled;
            await _Appointmentrepo.Update(idopont);
            return Results<Appointment>.Ok(idopont);

        }

        public async Task<Results<Appointment>> CreateAppointment(Appointment appointment)
        {
            if (appointment == null)
                return Results<Appointment>.Fail("Null az appointment");

            var hairdresser = await _Userrepo.GetById(appointment.HairdresserId);
            if (hairdresser == null || !hairdresser.Role.HasFlag(UserRole.Hairdresser))
                return Results<Appointment>.Fail("Válassz fodrász!");

            if (appointment.UserId == appointment.HairdresserId)
                return Results<Appointment>.Fail("Nem lehetsz saját magad vendége!😉");

            var user = await _Userrepo.GetById(appointment.UserId);
            if (user == null)
                return Results<Appointment>.Fail("Nem létezik ilyen felhasználó");

            if (await _Appointmentrepo.CountBookedByUserId(appointment.UserId) >= 3)
                return Results<Appointment>.Fail("Nem lehet több mint 3 lefoglalt időpont");

            var szolgaltatas = await _Servicerepo.GetById(appointment.ServiceId);
            if (szolgaltatas == null)
            {
                return Results<Appointment>.Fail("Válassz szolgáltatás!");
            }

            if (await _Appointmentrepo.ExistsInTimeRangeU(appointment.UserId, appointment.StartTime, appointment.EndTime))
                return Results<Appointment>.Fail("Nem lehet ugyan arra az időpontra 2 foglalásod");

            if (await _Appointmentrepo.ExistsInTimeRangeH(appointment.HairdresserId, appointment.StartTime, appointment.EndTime))
            {
                return Results<Appointment>.Fail($"Ez az időpont({appointment.StartTime.ToString("MM. dd. HH:mm")}) már foglalt");
            }
            await _Appointmentrepo.Create(appointment);
            return Results<Appointment>.Ok(appointment);
        }

        public async Task<Results<List<Appointment>>> GetHairdresserSchedule(int? hairdresserid, int offset)
        {
            if (hairdresserid == null)
                return Results<List<Appointment>>.Fail("Hibás id");

            var fodrasz = await _Userrepo.GetById(hairdresserid.Value);

            if (fodrasz == null || !fodrasz.Role.HasFlag(UserRole.Hairdresser))
                return Results<List<Appointment>>.Fail("Nincs ilyen fodrász");

            else
            {
                var idopontok = await _Appointmentrepo.GetAppointmentsByDateAndHairdresserBooked(fodrasz.UserId, DateOnly.FromDateTime(DateTime.Now).AddDays(offset));
                return Results<List<Appointment>>.Ok(idopontok);
            }
        }

        public async Task<Results<List<Appointment>>> GetUserAppointments(int? userid)
        {
            if (userid == null)
                return Results<List<Appointment>>.Fail("Hibás id");

            var dbUser = await _Userrepo.GetById(userid.Value);

            if (dbUser == null)
                return Results<List<Appointment>>.Fail("Nincs ilyen felhasználó");

            return Results<List<Appointment>>.Ok(await _Appointmentrepo.GetFutureAppointmentsByUser(userid.Value));
        }

        public async Task<Results<List<DateTime>>> GetAvailableSlots(int hairdresserId, DateOnly date, int serviceDurationInMinutes)
        {
            if (date.DayOfWeek == DayOfWeek.Sunday)
                return Results<List<DateTime>>.Fail("Vasárnap zárva vagyunk.");

            DateOnly cDay = DateOnly.FromDateTime(DateTime.Now);
            if (date < cDay)
                return Results<List<DateTime>>.Fail("Erre az időpontra már nem lehet foglalni!.");

            var appointments = await _Appointmentrepo.GetAppointmentsByDateAndHairdresser(hairdresserId, date);
            var ordered = appointments.Where(a => a.AppointmentStatus != AppointmentStatus.Cancelled)
                                      .OrderBy(a => a.StartTime)
                                      .ToList();

            var slots = new List<DateTime>();
            DateTime current;
            int nyitasOra = 8;
            int zarasOra = 18;

            if (cDay == date)
            {
                int kovetkezoOra = Math.Max(nyitasOra, DateTime.Now.Hour + 1);

                if (kovetkezoOra >= zarasOra)
                {
                    return Results<List<DateTime>>.Fail("Mára már nincs több szabad időpont.");
                }

                current = date.ToDateTime(new TimeOnly(kovetkezoOra, 0));
            }
            else
            {
                current = date.ToDateTime(new TimeOnly(nyitasOra, 0));
            }

            var closing = date.ToDateTime(new TimeOnly(zarasOra, 0));

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
                    current += TimeSpan.FromMinutes(serviceDurationInMinutes);
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

        public async Task<Results<Service>> CreateService(Service service)
        {
            var exists = await _Servicerepo.ExistsByName(service.Name);
            if (exists)
            {
                return Results<Service>.Fail("Már van ilyen szolgáltatás!");
            }

            var cService = await _Servicerepo.Create(service);
            return Results<Service>.Ok(cService);
        }

        public async Task<Results<Service>> UpdateService(Service service)
        {
            var existingService = await _Servicerepo.GetById(service.ServiceId);
            if (existingService == null)
            {
                return Results<Service>.Fail("A módosítani kívánt szolgáltatás már nem található az adatbázisban!");
            }

            if (existingService.Name != service.Name)
            {
                var nameConflict = await _Servicerepo.ExistsByName(service.Name);
                if (nameConflict)
                {
                    return Results<Service>.Fail("Már létezik másik szolgáltatás ezzel a névvel!");
                }
            }

            var updated = await _Servicerepo.Update(service);
            return Results<Service>.Ok(updated);
        }

        public async Task<Results<Appointment>> GetAppointmentById(int id)
        {
            var appointment = await _Appointmentrepo.GetById(id);

            if (appointment == null)
            {
                return Results<Appointment>.Fail("Nincs ilyen időpont!");
            }

            return Results<Appointment>.Ok(appointment);
        }

        public async Task<Results<List<Appointment>>> CancelAllAppointments(int? hairdresserid, int offset)
        {
            if (hairdresserid == null)
                return Results<List<Appointment>>.Fail("Hibás id");

            var fodrasz = await _Userrepo.GetById(hairdresserid.Value);

            if (fodrasz == null || !fodrasz.Role.HasFlag(UserRole.Hairdresser))
                return Results<List<Appointment>>.Fail("Nincs ilyen fodrász");

            var idopontok = await _Appointmentrepo.GetAppointmentsByDateAndHairdresser(fodrasz.UserId, DateOnly.FromDateTime(DateTime.Now).AddDays(offset));

            if (idopontok.Any())
            {
                try
                {
                    foreach (var idopont in idopontok)
                    {
                        if (idopont.AppointmentStatus != AppointmentStatus.Completed)
                        {
                            idopont.AppointmentStatus = AppointmentStatus.Cancelled;
                            _Appointmentrepo.UpdateWithoutSave(idopont);
                        }
                    }
                    await _Appointmentrepo.SaveAsync();

                    return Results<List<Appointment>>.Ok(idopontok);
                }
                catch (Exception ex)
                {
                    return Results<List<Appointment>>.Fail($"Hiba történt a mentés során: {ex.Message}");

                }
            }

            return Results<List<Appointment>>.Fail("Ezen a napon nincs foglalás!");
        }

        public async Task<int> AutoCompletePastAppointmentsAsync()
        {
            var idopontok = await _Appointmentrepo.GetPastAppointments();
            int count = idopontok.Count();

            if (count > 0)
            {
                try
                {
                    foreach (var idopont in idopontok)
                    {
                        idopont.AppointmentStatus = AppointmentStatus.Completed;
                        _Appointmentrepo.UpdateWithoutSave(idopont);
                    }
                    await _Appointmentrepo.SaveAsync();
                }
                catch (Exception)
                {
                    throw; // Továbbdobjuk a BackgroundService-nek, hogy ott logoljuk
                }
            }
            return count;
        }
    }
}
