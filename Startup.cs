using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using SJBRAssociatesWeb.Models;

namespace SJBRAssociatesWeb
{
    public class Startup
    {
        internal static void Configuration(WebApplicationBuilder builder, string[] args)
        {
            if (args.Length == 2 && args[0] == "hash")
            {
                Console.WriteLine(new PasswordHasher<object>().HashPassword(null!, args[1]));
                return;
            }

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Admin/Login";
                    options.AccessDeniedPath = "/Admin/Login";
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);
                    options.SlidingExpiration = true;
                });
            builder.Services.AddAuthorization();
            builder.Services.Configure<AdminAuthSettings>(builder.Configuration.GetSection("AdminAuth"));
        }
        internal static void RegisterServices(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<AppUser>();
        }
    }
}
