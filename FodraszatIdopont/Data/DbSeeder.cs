using FodraszatIdopont.Data;
using FodraszatIdopont.Helpers;
using FodraszatIdopont.Models.Entities;
using FodraszatIdopont.Models.Enums;

public static class DbSeeder
{
    public static void Seed(BarberDbContext context)
    {
        if (!context.Users.Any())
        {
            context.Users.AddRange(
                new User
                {
                    Name = "Petró Zoltán",
                    Email = "admin",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("admin"),
                    Role = UserRole.Admin | UserRole.Hairdresser | UserRole.User,
                    Sex = Gender.None,
                },
                new User
                {
                    Name = "Anna Kovács",
                    Email = "anna.kovacs@gmail.com",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("anna123"),
                    Role = UserRole.User,
                    Sex = Gender.Female,
                },
                new User
                {
                    Name = "Péter Nagy",
                    Email = "peter.nagy@gmail.com",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("peter123"),
                    Role = UserRole.User,
                    Sex = Gender.Male,
                },
                new User
                {
                    Name = "Nagy Marcell Miklós",
                    Email = "marcell.nagy@gmail.com",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("marcell123"),
                    Role = UserRole.User,
                    Sex = Gender.Male
                },
                new User
                {   
                    Name = "Nagy Gábor",
                    Email = "gabor.fodrasz@gmail.com",
                    ProfileImageUrl = "/images/profiles/091f0e39-a2a9-4345-90fc-a4a6e5f60b36.jpg",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("gabor123"),
                    Role = UserRole.Hairdresser | UserRole.User,
                    Sex = Gender.Male,
                },
                new User
                {
                    Name = "Belák Marcell",
                    Email = "marcell.fodrasz@gmail.com",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("marcell123"),
                    Role = UserRole.Hairdresser,
                    Sex = Gender.Male,
                },
                new User
                {
                    Name = "Tóth Eszter",
                    Email = "eszter.toth@gmail.com",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("eszter123"),
                    Role = UserRole.User,
                    Sex = Gender.Female,
                },
                new User
                {
                    Name = "Szabó Zsófia",
                    Email = "zsofia.szabo@gmail.com",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("zsofi123"),
                    Role = UserRole.User,
                    Sex = Gender.Female,
                },
                new User
                {
                    Name = "Kiss Viktória",
                    Email = "viki.kiss@gmail.com",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("viki123"),
                    Role = UserRole.User,
                    Sex = Gender.Female,
                },
                new User
                {
                    Name = "Farkas Réka",
                    Email = "reka.farkas@gmail.com",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("reka123"),
                    Role = UserRole.User,
                    Sex = Gender.Female,
                },
                new User
                {
                    Name = "Németh Lilla",
                    Email = "lilla.nemeth@gmail.com",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("lilla123"),
                    Role = UserRole.User,
                    Sex = Gender.Female,
                },
                new User
                {
                    Name = "Horváth Boglárka",
                    Email = "bogi.fodrasz@gmail.com",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("bogi123"),
                    Role = UserRole.Hairdresser,
                    Sex = Gender.Female,
                },
                new User
                {
                    Name = "Balogh Katalin",
                    Email = "kata.balogh@gmail.com",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("kata123"),
                    Role = UserRole.User,
                    Sex = Gender.Female,
                },
                new User
                {
                    Name = "Varga Máté",
                    Email = "mate.varga@gmail.com",
                    Phone = "36204444444",
                    PasswordHash = PasswordHelper.HashPassword("mate123"),
                    Role = UserRole.User,
                    Sex = Gender.Male,
                }
                );
            context.SaveChanges();
        }

        if (!context.Appointments.Any())
        {
            context.Services.AddRange(
                new Service
                {
                    Name = "Női hajvágás",
                    DurationInMinute = 60,
                    Price = 6000
                },
                new Service
                {
                    Name = "Férfi hajvágás",
                    DurationInMinute = 45,
                    Price = 4000
                },
                new Service
                {
                    Name = "Hajfestés",
                    DurationInMinute = 120,
                    Price = 15000
                },
                new Service
                {
                    Name = "Melírozás",
                    DurationInMinute = 90,
                    Price = 12000
                },
                new Service
                {
                    Name = "Frizura készítés",
                    DurationInMinute = 60,
                    Price = 7000
                }
            );
            context.SaveChanges();
        }

        if (!context.Services.Any())
        {
            context.Appointments.AddRange(
                // Teljesen foglalt nap UserId=4 fodrásznak: 2026. március 5. (10:00 - 18:00 folyamatos)
                new Appointment { UserId = 2, HairdresserId = 4, ServiceId = 2, StartTime = new DateTime(2026, 3, 5, 10, 0, 0), EndTime = new DateTime(2026, 3, 5, 10, 45, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Férfi hajvágás" },
                new Appointment { UserId = 3, HairdresserId = 4, ServiceId = 1, StartTime = new DateTime(2026, 3, 5, 10, 45, 0), EndTime = new DateTime(2026, 3, 5, 11, 45, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Női hajvágás" },
                new Appointment { UserId = 2, HairdresserId = 4, ServiceId = 3, StartTime = new DateTime(2026, 3, 5, 11, 45, 0), EndTime = new DateTime(2026, 3, 5, 13, 45, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Hajfestés" },
                new Appointment { UserId = 3, HairdresserId = 4, ServiceId = 4, StartTime = new DateTime(2026, 3, 5, 13, 45, 0), EndTime = new DateTime(2026, 3, 5, 15, 15, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Melírozás" },
                new Appointment { UserId = 2, HairdresserId = 4, ServiceId = 5, StartTime = new DateTime(2026, 3, 5, 15, 15, 0), EndTime = new DateTime(2026, 3, 5, 16, 15, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Frizura készítés" },
                new Appointment { UserId = 3, HairdresserId = 4, ServiceId = 2, StartTime = new DateTime(2026, 3, 5, 16, 15, 0), EndTime = new DateTime(2026, 3, 5, 17, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Férfi hajvágás" },
                new Appointment { UserId = 2, HairdresserId = 4, ServiceId = 1, StartTime = new DateTime(2026, 3, 5, 17, 0, 0), EndTime = new DateTime(2026, 3, 5, 18, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Női hajvágás" },

                // Részben foglalt nap UserId=5 fodrásznak: március 10. (csak délután foglalt)
                new Appointment { UserId = 3, HairdresserId = 5, ServiceId = 3, StartTime = new DateTime(2026, 3, 10, 14, 0, 0), EndTime = new DateTime(2026, 3, 10, 16, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Hajfestés délután" },
                new Appointment { UserId = 2, HairdresserId = 5, ServiceId = 4, StartTime = new DateTime(2026, 3, 10, 16, 0, 0), EndTime = new DateTime(2026, 3, 10, 17, 30, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Melírozás" },

                // Tolódásos nap  március 20. (reggel 120 perc, utána tolódik)
                new Appointment { UserId = 3, HairdresserId = 4, ServiceId = 3, StartTime = new DateTime(2026, 3, 20, 10, 0, 0), EndTime = new DateTime(2026, 3, 20, 12, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Reggeli hajfestés" },
                new Appointment { UserId = 2, HairdresserId = 4, ServiceId = 1, StartTime = new DateTime(2026, 3, 20, 13, 0, 0), EndTime = new DateTime(2026, 3, 20, 14, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Délutáni női hajvágás" },

                // Szabad napok: incs semmi (automatikusan szabad)
                // Extra részben  fodrásznak: március 15.
                new Appointment { UserId = 3, HairdresserId = 5, ServiceId = 5, StartTime = new DateTime(2026, 3, 15, 10, 0, 0), EndTime = new DateTime(2026, 3, 15, 11, 0, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Frizura reggel" },
                new Appointment { UserId = 2, HairdresserId = 5, ServiceId = 2, StartTime = new DateTime(2026, 3, 15, 14, 0, 0), EndTime = new DateTime(2026, 3, 15, 14, 45, 0), AppointmentStatus = AppointmentStatus.Booked, Notes = "Férfi hajvágás délután" }
            );
            context.SaveChanges();
        }
    }
}
