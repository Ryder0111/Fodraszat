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
    // Teljesen foglalt nap UserId=4 fodrásznak: 2026. március 5. (10:00 - 18:00 folyamatos)
    new Appointment { AppointmentId = 1, UserId = 2, HairdresserId = 4, ServiceId = 2, StartTime = new DateTime(2026, 3, 5, 10, 0, 0), EndTime = new DateTime(2026, 3, 5, 10, 45, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Férfi hajvágás" },
    new Appointment { AppointmentId = 2, UserId = 3, HairdresserId = 4, ServiceId = 1, StartTime = new DateTime(2026, 3, 5, 10, 45, 0), EndTime = new DateTime(2026, 3, 5, 11, 45, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Női hajvágás" },
    new Appointment { AppointmentId = 3, UserId = 2, HairdresserId = 4, ServiceId = 3, StartTime = new DateTime(2026, 3, 5, 11, 45, 0), EndTime = new DateTime(2026, 3, 5, 13, 45, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Hajfestés" },
    new Appointment { AppointmentId = 4, UserId = 3, HairdresserId = 4, ServiceId = 4, StartTime = new DateTime(2026, 3, 5, 13, 45, 0), EndTime = new DateTime(2026, 3, 5, 15, 15, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Melírozás" },
    new Appointment { AppointmentId = 5, UserId = 2, HairdresserId = 4, ServiceId = 5, StartTime = new DateTime(2026, 3, 5, 15, 15, 0), EndTime = new DateTime(2026, 3, 5, 16, 15, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Frizura készítés" },
    new Appointment { AppointmentId = 6, UserId = 3, HairdresserId = 4, ServiceId = 2, StartTime = new DateTime(2026, 3, 5, 16, 15, 0), EndTime = new DateTime(2026, 3, 5, 17, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Férfi hajvágás" },
    new Appointment { AppointmentId = 7, UserId = 2, HairdresserId = 4, ServiceId = 1, StartTime = new DateTime(2026, 3, 5, 17, 0, 0), EndTime = new DateTime(2026, 3, 5, 18, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Női hajvágás" },

    // Részben foglalt nap UserId=5 fodrásznak: március 10. (csak délután foglalt)
    new Appointment { AppointmentId = 8, UserId = 3, HairdresserId = 5, ServiceId = 3, StartTime = new DateTime(2026, 3, 10, 14, 0, 0), EndTime = new DateTime(2026, 3, 10, 16, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Hajfestés délután" },
    new Appointment { AppointmentId = 9, UserId = 2, HairdresserId = 5, ServiceId = 4, StartTime = new DateTime(2026, 3, 10, 16, 0, 0), EndTime = new DateTime(2026, 3, 10, 17, 30, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Melírozás" },

    // Tolódásos nap UserId=4 fodrásznak: március 20. (reggel 120 perc, utána tolódik)
    new Appointment { AppointmentId = 10, UserId = 3, HairdresserId = 4, ServiceId = 3, StartTime = new DateTime(2026, 3, 20, 10, 0, 0), EndTime = new DateTime(2026, 3, 20, 12, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Reggeli hajfestés" },
    new Appointment { AppointmentId = 11, UserId = 2, HairdresserId = 4, ServiceId = 1, StartTime = new DateTime(2026, 3, 20, 13, 0, 0), EndTime = new DateTime(2026, 3, 20, 14, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Délutáni női hajvágás" },

    // Szabad napok: március 12., 17. – nincs semmi (automatikusan szabad)
    // Extra részben foglalt nap UserId=5 fodrásznak: március 15.
    new Appointment { AppointmentId = 12, UserId = 3, HairdresserId = 5, ServiceId = 5, StartTime = new DateTime(2026, 3, 15, 10, 0, 0), EndTime = new DateTime(2026, 3, 15, 11, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Frizura reggel" },
    new Appointment { AppointmentId = 13, UserId = 2, HairdresserId = 5, ServiceId = 2, StartTime = new DateTime(2026, 3, 15, 14, 0, 0), EndTime = new DateTime(2026, 3, 15, 14, 45, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Férfi hajvágás délután" }
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
