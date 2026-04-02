using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Services.Interface
{
    public interface IAppointmentService
    {
        Task<Results<List<User>>> GetAllHairdressers();

        Task<Results<List<Service>>> GetAllServices();

        Task<Results<Appointment>> GetAppointmentById(int id);

        Task<Results<Appointment>> CreateAppointment(Appointment appointment);

        Task<Results<Appointment>> CancelAppointment(int appointmentid);

        Task<Results<Appointment>> CompleteAppointment(int appointmentid);

        Task<Results<List<Appointment>>> GetUserAppointments(int? userid);

        Task<Results<List<Appointment>>> GetHairdresserSchedule(int? hairdresserid);

        Task<Results<List<DateTime>>> GetAvailableSlots(int hairdresserId, DateOnly date, int serviceDurationInMinutes);

        Task<Results<List<DateOnly>>> GetBookedDays(int hairdresserId, DateOnly startDate, DateOnly endDate);

        Task<Results<Service>> GetServiceById(int serviceId);

        Task<Results<Service>> CreateService(Service service);

        Task<Results<Service>> UpdateService(Service service);
    }
}