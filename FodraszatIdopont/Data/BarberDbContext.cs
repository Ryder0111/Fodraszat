using FodraszatIdopont.Models.Entities;
using Microsoft.EntityFrameworkCore;
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
        }
    }
}
