using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Services.Interface
{
    public interface IAppointmentService
    {
        Task<Results<List<User>>> GetAllHairdressers();

        Task<Results<List<Service>>> GetAllServices();

        Task<Results<Appointment>> CreateAppointment(Appointment appointment);

        Task<Results<Appointment>> CancelAppointment(Appointment appointment);

        Task<Results<List<Appointment>>> GetUserAppointments(User user);

        Task<Results<List<Appointment>>> GetHairdresseSchedule(User hairdresser);
    }
}