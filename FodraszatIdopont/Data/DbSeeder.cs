using FodraszatIdopont.Data;
using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.Enums;
using System;
using System.Linq;

public static class DbSeeder
{
    public static void Seed(BarberDbContext context)
    {
        // 1. FELHASZNÁLÓK SEEDELÉSE
        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User {IsEmailVerified = true, Name = "Petró Zoltán", Email = "admin", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("admin"), Role = UserRole.Admin | UserRole.Hairdresser | UserRole.User, Sex = Gender.None },
                new User {IsEmailVerified = true, Name = "Anna Kovács", Email = "anna.kovacs@gmail.com", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("anna123"), Role = UserRole.User, Sex = Gender.Female },
                new User {IsEmailVerified = true, Name = "Péter Nagy", Email = "peter.nagy@gmail.com", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("peter123"), Role = UserRole.User, Sex = Gender.Male },
                new User {IsEmailVerified = true, Name = "Nagy Marcell Miklós", Email = "marcell.nagy@gmail.com", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("marcell123"), Role = UserRole.User, Sex = Gender.Male },
                new User {IsEmailVerified = true, Name = "Nagy Gábor", Email = "gabor.fodrasz@gmail.com", ProfileImageUrl = "/images/profiles/091f0e39-a2a9-4345-90fc-a4a6e5f60b36.jpg", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("gabor123"), Role = UserRole.Hairdresser | UserRole.User, Sex = Gender.Male },
                new User {IsEmailVerified = true, Name = "Belák Marcell", Email = "marcell.fodrasz@gmail.com", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("marcell123"), Role = UserRole.Hairdresser, Sex = Gender.Male },
                new User {IsEmailVerified = true, Name = "Tóth Eszter", Email = "eszter.toth@gmail.com", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("eszter123"), Role = UserRole.User, Sex = Gender.Female },
                new User {IsEmailVerified = true, Name = "Szabó Zsófia", Email = "zsofia.szabo@gmail.com", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("zsofi123"), Role = UserRole.User, Sex = Gender.Female },
                new User {IsEmailVerified = true, Name = "Kiss Viktória", Email = "viki.kiss@gmail.com", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("viki123"), Role = UserRole.User, Sex = Gender.Female },
                new User {IsEmailVerified = true, Name = "Farkas Réka", Email = "reka.farkas@gmail.com", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("reka123"), Role = UserRole.User, Sex = Gender.Female },
                new User {IsEmailVerified = true, Name = "Németh Lilla", Email = "lilla.nemeth@gmail.com", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("lilla123"), Role = UserRole.User, Sex = Gender.Female },
                new User {IsEmailVerified = true, Name = "Horváth Boglárka", Email = "bogi.fodrasz@gmail.com", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("bogi123"), Role = UserRole.Hairdresser, Sex = Gender.Female },
                new User {IsEmailVerified = true, Name = "Balogh Katalin", Email = "kata.balogh@gmail.com", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("kata123"), Role = UserRole.User, Sex = Gender.Female },
                new User {IsEmailVerified = true, Name = "Varga Máté", Email = "mate.varga@gmail.com", Phone = "36204444444", PasswordHash = PasswordHelper.HashPassword("mate123"), Role = UserRole.User, Sex = Gender.Male }
            );
            context.SaveChanges();
        }

        // 2. SZOLGÁLTATÁSOK SEEDELÉSE (Önálló feltétel!)
        if (!context.Services.Any())
        {
            context.Services.AddRange(
                new Service { Name = "Női hajvágás", DurationInMinute = 60, Price = 6000 },
                new Service { Name = "Férfi hajvágás", DurationInMinute = 45, Price = 4000 },
                new Service { Name = "Hajfestés", DurationInMinute = 120, Price = 15000 },
                new Service { Name = "Melírozás", DurationInMinute = 90, Price = 12000 },
                new Service { Name = "Frizura készítés", DurationInMinute = 60, Price = 7000 }
            );
            context.SaveChanges();
        }

        // 3. IDŐPONTOK SEEDELÉSE (Önálló feltétel!)
        if (!context.Appointments.Any())
        {
            context.Appointments.AddRange(
                // --- BELÁK MARCELL (ID=4) - JÚNIUS 8. TELJESEN FOGLALT (08:00 - 18:00) ---
                new Appointment { UserId = 2, HairdresserId = 6, ServiceId = 3, StartTime = new DateTime(2026, 6, 8, 8, 0, 0), EndTime = new DateTime(2026, 6, 8, 10, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Festés" },
                new Appointment { UserId = 3, HairdresserId = 6, ServiceId = 1, StartTime = new DateTime(2026, 6, 8, 10, 0, 0), EndTime = new DateTime(2026, 6, 8, 11, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Női vágás" },
                new Appointment { UserId = 2, HairdresserId = 6, ServiceId = 4, StartTime = new DateTime(2026, 6, 8, 11, 0, 0), EndTime = new DateTime(2026, 6, 8, 12, 30, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Melír" },
                new Appointment { UserId = 3, HairdresserId = 6, ServiceId = 2, StartTime = new DateTime(2026, 6, 8, 12, 30, 0), EndTime = new DateTime(2026, 6, 8, 13, 15, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Ebédidő helyett vágás" },
                new Appointment { UserId = 2, HairdresserId = 6, ServiceId = 3, StartTime = new DateTime(2026, 6, 8, 13, 15, 0), EndTime = new DateTime(2026, 6, 8, 15, 15, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Festés 2" },
                new Appointment { UserId = 3, HairdresserId = 6, ServiceId = 1, StartTime = new DateTime(2026, 6, 8, 15, 15, 0), EndTime = new DateTime(2026, 6, 8, 16, 15, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Női vágás 2" },
                new Appointment { UserId = 2, HairdresserId = 6, ServiceId = 5, StartTime = new DateTime(2026, 6, 8, 16, 15, 0), EndTime = new DateTime(2026, 6, 8, 17, 15, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Frizura" },
                new Appointment { UserId = 3, HairdresserId = 6, ServiceId = 2, StartTime = new DateTime(2026, 6, 8, 17, 15, 0), EndTime = new DateTime(2026, 6, 8, 18, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Utolsó férfi vágás" },

                // --- BELÁK MARCELL (ID=4) - JÚNIUS 15. IS TELJESEN FOGLALT ---
                new Appointment { UserId = 2, HairdresserId = 6, ServiceId = 3, StartTime = new DateTime(2026, 6, 18, 8, 0, 0), EndTime = new DateTime(2026, 6, 15, 10, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Reggeli nagy munka" },
                new Appointment { UserId = 3, HairdresserId = 6, ServiceId = 4, StartTime = new DateTime(2026, 6, 18, 10, 0, 0), EndTime = new DateTime(2026, 6, 15, 11, 30, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Melír" },
                new Appointment { UserId = 2, HairdresserId = 6, ServiceId = 3, StartTime = new DateTime(2026, 6, 18, 11, 30, 0), EndTime = new DateTime(2026, 6, 15, 13, 30, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Hosszú festés" },
                new Appointment { UserId = 3, HairdresserId = 6, ServiceId = 1, StartTime = new DateTime(2026, 6, 18, 13, 30, 0), EndTime = new DateTime(2026, 6, 15, 14, 30, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Vágás" },
                new Appointment { UserId = 2, HairdresserId = 6, ServiceId = 3, StartTime = new DateTime(2026, 6, 18, 14, 30, 0), EndTime = new DateTime(2026, 6, 15, 16, 30, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Délutáni festés" },
                new Appointment { UserId = 3, HairdresserId = 6, ServiceId = 4, StartTime = new DateTime(2026, 6, 18, 16, 30, 0), EndTime = new DateTime(2026, 6, 15, 18, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Zárás melírral" }
            );
            context.SaveChanges();
        }
    }
}