using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyDefenceSistem.Data;
using MyDefenceSistem.BL;
using MyDefenceSistem.Sockets;
using System.Configuration;
using MyDefenceSistem.DAL;
namespace MyDefenceSistem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<MyDefenceSistemContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyDefenceSistemContext") ?? throw new InvalidOperationException("Connection string 'MyDefenceSistemContext' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IThreatsService, ThreatsService>();
            builder.Services.AddScoped<IWeaponsService, WeaponsService>();
            builder.Services.AddScoped<IoriginService, OriginService>();
            builder.Services.AddScoped<IoriginTable, OriginTable>();
            builder.Services.AddScoped<IWeaponsTable, WeaponsTable>();
            builder.Services.AddScoped<IThreatTable, ThreatTable>();
            var app = builder.Build();

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

            app.UseAuthorization();
            app.MapHub<TreatHub>("/threat_hub");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
