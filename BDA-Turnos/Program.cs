using BDA_Turnos.Repositorio;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TurnosDBContext>(options =>options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddScoped<RepositorioUsuarios>();
builder.Services.AddScoped<RepositorioEspecialidades>();
builder.Services.AddScoped<RepositorioMedicos>();
builder.Services.AddScoped<RepositorioTurnos>();

//para las cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/ControladoraSesion/Index"; //si no inicio
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.AccessDeniedPath = "/ControladoraSesion/Index";
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

var culturaEsAr = new CultureInfo("es-AR");

//Formato de números con punto decimal
culturaEsAr.NumberFormat.NumberDecimalSeparator = ".";
culturaEsAr.NumberFormat.CurrencyDecimalSeparator = ".";

// cultura por defecto
CultureInfo.DefaultThreadCurrentCulture = culturaEsAr;
CultureInfo.DefaultThreadCurrentUICulture = culturaEsAr;

// Zona horaria Argentina/Buenos Aires
try
{
    TimeZoneInfo timeZoneBsAs = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
    TimeZoneInfo.ClearCachedData();
}
catch
{
    TimeZoneInfo timeZoneBsAs = TimeZoneInfo.FindSystemTimeZoneById("America/Argentina/Buenos_Aires");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ControladoraSesion}/{action=Index}/{id?}");

app.Run();