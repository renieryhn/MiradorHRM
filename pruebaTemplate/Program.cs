using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PlanillaPM.Models;
using PlanillaPM.Permission;
using PlanillaPM.Services;
using PlanillaPM.Servicio;
using Syncfusion.Licensing;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using PlanillaPM.Constants;
using System.Security.Claims;



var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("sDBConnection") ?? throw new InvalidOperationException("Connection string 'PlanillaContextConnection' not found.");




var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

builder.Services.AddControllersWithViews()
        .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

//Roles

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddIdentity<Usuario, IdentityRole>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = true;
    opciones.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
})
.AddEntityFrameworkStores<PlanillaContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo(Config.enUSCulture)
    };

    options.DefaultRequestCulture = new RequestCulture(Config.enUSCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
    {
        // My custom request culture logic
        return await Task.FromResult(new ProviderCultureResult(Config.enUSCulture));
    }));
});

// Add services to the container.
builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));
}).AddRazorRuntimeCompilation();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<PlanillaContext>(opciones =>
    opciones.UseSqlServer("name=sDBConnection"));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<PlanillaContext>();


//builder.Services.AddAuthentication().AddMicrosoftAccount(opciones =>
//{
//    opciones.ClientId = builder.Configuration["MicrosoftClientId"];
//    opciones.ClientSecret = builder.Configuration["MicrosoftSecretId"];
//});

//builder.Services.AddIdentity<Usuario, IdentityRole>(opciones =>
//{
//    opciones.SignIn.RequireConfirmedAccount = true;
//    opciones.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

//}).AddEntityFrameworkStores<PlanillaContext>()
//.AddDefaultTokenProviders();

//builder.Services.AddScoped<UserManager<Usuario>>();

builder.Services.AddTransient<EmailService, EmailService>();

//Utilizar nuestras propias ventanas
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
    Opciones =>
    {
        Opciones.LoginPath = "/usuario/login";
        Opciones.AccessDeniedPath = "/usuario/login";
    });




builder.Services.AddMemoryCache();
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
builder.Services.AddResponseCompression();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".PMPlanilla.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.Configure<IdentityOptions>(options =>
{
    // Configurar requisitos de contraseña
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // Otros ajustes opcionales...
});

var app = builder.Build();

//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBPh8sVXJxS0d+X1RPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9nSXpTdERkXHhdcHZVQGY=");
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBMAY9C3t2U1hhQlJBfVhdXGJWfFN0QXNYflRwcl9EZEwgOX1dQl9nSXhSd0RhXHdacXNXTmI=");
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger("app");

try
{
    var userManager = app.Services.GetRequiredService<UserManager<Usuario>>();
    var roleManager = app.Services.GetRequiredService<RoleManager<IdentityRole>>();
    await PlanillaPM.Seeds.DefaultRoles.SeedAsync(userManager, roleManager);
    //await PlanillaPM.Seeds.DefaultUsers.SeedBasicUserAsync(userManager, roleManager);
    await PlanillaPM.Seeds.DefaultUsers.SeedSuperAdminAsync(userManager, roleManager);
    logger.LogInformation("Finished Seeding Default Data");
    logger.LogInformation("Application Starting");
}
catch (Exception ex)
{
    logger.LogWarning(ex, "An error occurred seeding the DB");
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
