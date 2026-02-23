using FodraszatIdopont.Models.Entities;

namespace FodraszatIdopont.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment> Create(Appointment appointment);

        Task Update(Appointment appointment);

        Task<Appointment?> GetById(int id);

        Task<List<Appointment>> GetByUserId(int id);

        Task<List<Appointment>> GetByHairdresserId(int id);

        Task<List<Appointment>> GetAppointmentsByHairdresserInTimeRange(int id, DateTime start, DateTime end);

        Task<List<Appointment>> GetFutureAppointmentsByUser(int id);

        Task<List<Appointment>> GetAppointmentsByDateAndHairdresser(int id, DateOnly date);

        Task<bool> ExistsInTimeRange(int id, DateTime start, DateTime end);

    }
}