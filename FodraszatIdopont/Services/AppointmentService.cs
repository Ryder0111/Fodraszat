using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services.Interface;

namespace FodraszatIdopont.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;

        public AppointmentService(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        public Task<Results<Appointment>> CancelAppointment(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public Task<Results<Appointment>> CreateAppointment(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public Task<Results<List<Appointment>>> GetHairdresseSchedule(Hairdresser hairdresser)
        {
            throw new NotImplementedException();
        }

        public Task<Results<List<Appointment>>> GetUserAppointments(User user)
        {
            throw new NotImplementedException();
        }
    }
}
