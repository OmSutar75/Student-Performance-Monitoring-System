using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentPerformanceManagment.Models;
using EmailService;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace StudentPerformanceManagment
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer("Server=(LocalDB)\\MSSQLLocalDB;Database=SPMSDB;Trusted_Connection=True;"));


            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("smtp"));

            builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();


            var app = builder.Build();


            //seed
            using (var scope = app.Services.CreateScope())
            {
                await IdentitySeed.SeedRolesAndAdmin(scope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=login}/{id?}");

            app.Run();
        }
    }
}
