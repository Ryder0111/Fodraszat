using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Services.Interface
{
    public interface IAppointmentService
    {
        Task<Results<Appointment>> CreateAppointment(Appointment appointment, int ServiceId);

        Task<Results<Appointment>> CancelAppointment(Appointment appointment);

        Task<Results<List<Appointment>>> GetUserAppointments(User user);

        Task<Results<List<Appointment>>> GetHairdresseSchedule(User hairdresser);
    }
}