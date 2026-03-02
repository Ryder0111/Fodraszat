using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
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
                    Name = "Eszter Fodrász",
                    Email = "eszter.fodrasz@gmail.com",
                    PasswordHash = PasswordHelper.HashPassword("eszter123"),
                    Role = Models.Enums.UserRole.Hairdresser,
                    Sex = Models.Enums.Gender.Famale,
                },
                new User
                {
                    UserId = 5,
                    Name = "Gábor Fodrász",
                    Email = "gabor.fodrasz@gmail.com",
                    PasswordHash = PasswordHelper.HashPassword("gabor123"),
                    Role = Models.Enums.UserRole.Hairdresser,
                    Sex = Models.Enums.Gender.Male,
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
