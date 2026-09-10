using SJBRAssociatesWeb;
using SJBRAssociatesWeb.Services;

var builder = WebApplication.CreateBuilder(args);
Startup.Configuration(builder, args);
builder.Services.AddControllersWithViews();

// Our "database": a JSON file reader/writer. Registered as a singleton because
// it holds no per-request state; internal locking makes concurrent access safe.
builder.Services.AddSingleton<IJsonDataService, JsonDataService>();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
