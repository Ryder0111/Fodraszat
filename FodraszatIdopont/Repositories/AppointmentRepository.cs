using FodraszatIdopont.Data;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FodraszatIdopont.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly BarberDbContext _db;

        public AppointmentRepository(BarberDbContext db)
        {
            _db = db;
        }

        public async Task<int> CountBookedByUserId(int userId)
        {
            return await _db.Appointments.CountAsync(a => a.UserId == userId && a.AppointmentStatus == Models.Enums.AppointmentStatus.Booked);
        }

        public async Task<Appointment> Create(Appointment appointment)
        {
            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();
            return appointment;
        }

        public async Task<bool> ExistsInTimeRangeH(int id, DateTime start, DateTime end)
        {
            return await _db.Appointments.Where(h => h.HairdresserId == id).AnyAsync(a => start < a.EndTime && end > a.StartTime);
        }

        public async Task<bool> ExistsInTimeRangeU(int id, DateTime start, DateTime end)
        {
            return await _db.Appointments.Where(u => u.UserId == id).AnyAsync(a => start < a.EndTime && end > a.StartTime);
        }


        public async Task<List<Appointment>> GetAppointmentsByDateAndHairdresser(int id, DateOnly date)
        {
            return await _db.Appointments.Where(h => h.HairdresserId == id && h.StartTime >= date.ToDateTime(TimeOnly.MinValue) && h.StartTime < date.ToDateTime(TimeOnly.MinValue).AddDays(1)).Include(s => s.Service).Include(u => u.User).ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByHairdresserInTimeRange(int id, DateTime start, DateTime end)
        {
            return await _db.Appointments.Where(a => a.HairdresserId == id && start < a.EndTime && end > a.StartTime).ToListAsync();
        }


        public async Task<List<Appointment>> GetByHairdresserId(int id)
        {
            return await _db.Appointments.Where(h => h.HairdresserId == id).ToListAsync();
        }
        public async Task<List<Appointment>> GetFutureAppointmentsByHairdresserId(int id)
        {
            return await _db.Appointments.Where(u => u.HairdresserId == id && u.StartTime > DateTime.Now).ToListAsync();
        }

        public async Task<Appointment?> GetById(int id)
        {
            return await _db.Appointments.SingleOrDefaultAsync(a => a.AppointmentId == id);
        }

        public async Task<List<Appointment>> GetByUserId(int id)
        {
            return await _db.Appointments.Where(u => u.UserId == id).ToListAsync();
        }

        public async Task<List<Appointment>> GetFutureAppointmentsByUser (int id)
        {
            return await _db.Appointments.Where(u => u.UserId == id && u.StartTime > DateTime.Now).Include(a => a.Service).Include(h => h.Hairdresser).ToListAsync();
        }

        public async Task Update(Appointment appointment)
        {
            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
        }
    }
}