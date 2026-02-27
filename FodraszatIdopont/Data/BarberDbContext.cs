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
                }
            );
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
    }
}
