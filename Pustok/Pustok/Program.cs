using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Database;
using Pustok.Services.Abstracts;
using Pustok.Services.Concretes;
using Pustok.Hubs;

namespace Pustok;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Services
        builder.Services
            .AddControllersWithViews()
            .AddRazorRuntimeCompilation();

        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(o =>
            {
                o.Cookie.Name = "CeyhunIdentity";
                o.LoginPath = "/auth/login";
            });

        builder.Services
            .AddDbContext<PustokDbContext>(o =>
            {
                o.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
            })
            .AddScoped<IUserService, UserService>()
            .AddSingleton<IFileService, ServerFileService>()
            .AddScoped<IEmailService, MailkitEmailService>()
            .AddScoped<IOrderService, OrderService>()
            .AddScoped<INotificationService, NotificationService>()
            .AddScoped<IBasketService, BasketService>()
            .AddScoped<IUserActivationService, UserActivationService>()
            .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
            .AddHttpContextAccessor()
            .AddHttpClient()
            .AddSingleton<IAlertMessageService, AlertMessageService>()
            .AddSingleton<OnlineUserTracker>()
            .AddSingleton<IOrderStatusNotificationService, OrderStatusNotificationService>();
            builder.Services
            .AddSignalR();

        var app = builder.Build();

        //Middleware (Chain of responsibily)
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute("default", "{controller=Home}/{action=Index}");

        app.MapHub<AlertMessageHub>("/alert-hub"); //hub endpoint, web-socket endpoint
        app.MapHub<OnlineUserHub>("/online-user-hub"); //hub endpoint, web-socket endpoint
        app.MapHub<StaffUsersViewHub>("/staff-users-view-hub"); //hub endpoint, web-socket endpoint
        app.MapHub<OrderStatusNotificationHub>("/order-status-notification-hub");  // Hub or Web-Socket endpoint...

        app.Run();
    }
}