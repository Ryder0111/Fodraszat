using FodraszatIdopont.Data;
using FodraszatIdopont.Models;
using FodraszatIdopont.Repositories;
using FodraszatIdopont.Repositories.Interfaces;
using FodraszatIdopont.Services;
using FodraszatIdopont.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FodraszatIdopont
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<BarberDbContext>(
                options => options
                .UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Database=BarberDB;Trusted_Connection=True"));



            builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.Cookie.Name = "FodraszatAuth";
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/Login";
            });

            //Szolgáltatások (interface,class)
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BarberDbContext>();
                db.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAntiforgery();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
