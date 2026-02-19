using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Repositories.Interfaces;

namespace FodraszatIdopont.Repositories
{
    public class AppointmentService : IAppointmentRepository
    {
        public Task<Appointment> Creat(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public Task<List<Appointment>> GetAppointmentsByDateAndHairdresser(int id, DateOnly date)
        {
            throw new NotImplementedException();
        }

        public Task<List<Appointment>> GetAppointmentsByHairdresserInTimeRange(int id, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public Task<List<Appointment>> GetByHairdresserId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Appointment> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Appointment>> GetByUserId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Appointment>> GetFutureAppointmentsByUser(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Appointment> Update(Appointment appointment)
        {
            throw new NotImplementedException();
        }
    }
}
