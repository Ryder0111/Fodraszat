using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection.Emit;

namespace FodraszatIdopont.Data
{
    public class BarberDbContext : DbContext
    {
        public BarberDbContext(DbContextOptions<BarberDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Kapcsolatok beállítása
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Hairdresser)
                .WithMany(u => u.HairdresserAppointments)
                .HasForeignKey(a => a.HairdresserId)
                .OnDelete(DeleteBehavior.Restrict);


            //Email egyediség
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>().HasData
            (
                new User
                {
                    UserId = 1,
                    Name = "admin",
                    Email = "admin",
                    PasswordHash = PasswordHelper.HashPassword("admin"),
                    Role = Models.Enums.UserRole.Admin,
                    Sex = Models.Enums.Gender.None,
                },
                new User
                {
                    UserId = 2,
                    Name = "Anna Kovács",
                    Email = "anna.kovacs@gmail.com",
                    PasswordHash = PasswordHelper.HashPassword("anna123"),
                    Role = Models.Enums.UserRole.User,
                    Sex = Models.Enums.Gender.Famale,
                },
                new User
                {
                    UserId = 3,
                    Name = "Péter Nagy",
                    Email = "peter.nagy@gmail.com",
                    PasswordHash = PasswordHelper.HashPassword("peter123"),
                    Role = Models.Enums.UserRole.User,
                    Sex = Models.Enums.Gender.Male,
                },
                new User
                {
                    UserId = 4,
                    Name = "Nagy Gábor",
                    Email = "gabor.fodrasz@gmail.com",
                    PasswordHash = PasswordHelper.HashPassword("gabor123"),
                    Role = Models.Enums.UserRole.Hairdresser,
                    Sex = Models.Enums.Gender.Famale,
                },
                new User
                {
                    UserId = 5,
                    Name = "Belák Marcell",
                    Email = "marcell.fodrasz@gmail.com",
                    PasswordHash = PasswordHelper.HashPassword("marcell123"),
                    Role = Models.Enums.UserRole.Hairdresser,
                    Sex = Models.Enums.Gender.Male,
                }
            );

            //Időpontok
            modelBuilder.Entity<Appointment>().HasData(
            new Appointment
            {
                AppointmentId = 1,
                UserId = 1,
                HairdresserId = 2,
                ServiceId = 1,
                StartTime = new DateTime(2026, 4, 10, 9, 0, 0),
                EndTime = new DateTime(2026, 4, 10, 9, 30, 0),
                AppointmentStatus = AppointmentStatus.Booked
            },
            new Appointment
            {
                AppointmentId = 2,
                UserId = 2,
                HairdresserId = 2,
                ServiceId = 2,
                StartTime = new DateTime(2026, 4, 10, 10, 0, 0),
                EndTime = new DateTime(2026, 4, 10, 11, 0, 0),
                AppointmentStatus = AppointmentStatus.Booked
            },
            new Appointment
            {
                AppointmentId = 3,
                UserId = 3,
                HairdresserId = 3,
                ServiceId = 1,
                StartTime = new DateTime(2025, 12, 15, 8, 30, 0),
                EndTime = new DateTime(2025, 12, 15, 9, 0, 0),
                AppointmentStatus = AppointmentStatus.Completed
            },
            new Appointment
            {
                AppointmentId = 4,
                UserId = 4,
                HairdresserId = 3,
                ServiceId = 3,
                StartTime = new DateTime(2025, 11, 20, 9, 30, 0),
                EndTime = new DateTime(2025, 11, 20, 10, 30, 0),
                AppointmentStatus = AppointmentStatus.Cancelled
            },
            new Appointment
            {
                AppointmentId = 5,
                UserId = 1,
                HairdresserId = 2,
                ServiceId = 2,
                StartTime = new DateTime(2026, 4, 12, 13, 0, 0),
                EndTime = new DateTime(2026, 4, 12, 14, 0, 0),
                AppointmentStatus = AppointmentStatus.Booked
            },
            new Appointment
            {
                AppointmentId = 6,
                UserId = 5,
                HairdresserId = 3,
                ServiceId = 1,
                StartTime = new DateTime(2025, 10, 5, 14, 30, 0),
                EndTime = new DateTime(2025, 10, 5, 15, 0, 0),
                AppointmentStatus = AppointmentStatus.Completed
            },
            new Appointment
            {
                AppointmentId = 7,
                UserId = 2,
                HairdresserId = 4,
                ServiceId = 3,
                StartTime = new DateTime(2026, 4, 13, 10, 0, 0),
                EndTime = new DateTime(2026, 4, 13, 11, 30, 0),
                AppointmentStatus = AppointmentStatus.Booked
            },
            new Appointment
            {
                AppointmentId = 8,
                UserId = 3,
                HairdresserId = 4,
                ServiceId = 2,
                StartTime = new DateTime(2025, 9, 18, 12, 0, 0),
                EndTime = new DateTime(2025, 9, 18, 13, 0, 0),
                AppointmentStatus = AppointmentStatus.Completed
            },
            new Appointment
            {
                AppointmentId = 9,
                UserId = 4,
                HairdresserId = 2,
                ServiceId = 1,
                StartTime = new DateTime(2026, 4, 14, 9, 0, 0),
                EndTime = new DateTime(2026, 4, 14, 9, 30, 0),
                AppointmentStatus = AppointmentStatus.Booked
            },
            new Appointment
            {
                AppointmentId = 10,
                UserId = 5,
                HairdresserId = 3,
                ServiceId = 3,
                StartTime = new DateTime(2025, 8, 22, 11, 0, 0),
                EndTime = new DateTime(2025, 8, 22, 12, 30, 0),
                AppointmentStatus = AppointmentStatus.Cancelled
            }
        );

            // Services
            modelBuilder.Entity<Service>().HasData(
                new Service
                {
                    ServiceId = 1,
                    Name = "Női hajvágás",
                    DurationInMinute = 60,
                    Price = 6000
                },
                new Service
                {
                    ServiceId = 2,
                    Name = "Férfi hajvágás",
                    DurationInMinute = 45,
                    Price = 4000
                },
                new Service
                {
                    ServiceId = 3,
                    Name = "Hajfestés",
                    DurationInMinute = 120,
                    Price = 15000
                },
                new Service
                {
                    ServiceId = 4,
                    Name = "Melírozás",
                    DurationInMinute = 90,
                    Price = 12000
                },
                new Service
                {
                    ServiceId = 5,
                    Name = "Frizura készítés",
                    DurationInMinute = 60,
                    Price = 7000
                }
            );
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
    }
}
