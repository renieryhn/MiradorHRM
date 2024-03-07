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
using PlanillaPM.Services;
using PlanillaPM.Servicio;
using System.Configuration;




var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("sDBConnection") ?? throw new InvalidOperationException("Connection string 'PlanillaContextConnection' not found.");

// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services.AddDbContext<PlanillaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("sDBConnection")));

var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

// Add services to the container.
builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));
});

builder.Services.AddTransient<IRepositorioRoles, RepositorioRoles>();
builder.Services.AddTransient<IUserStore<TipoRol>, UsuarioStore>();
builder.Services.AddIdentityCore<TipoRol>();

builder.Services.AddDbContext<PlanillaContext>(opciones =>
    opciones.UseSqlServer("name=sDBConnection"));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<PlanillaContext>();


//builder.Services.AddAuthentication().AddMicrosoftAccount(opciones =>
//{
//    opciones.ClientId = builder.Configuration["MicrosoftClientId"];
//    opciones.ClientSecret = builder.Configuration["MicrosoftSecretId"];
//});

builder.Services.AddIdentity<Usuario, IdentityRole>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = true;
    opciones.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

}).AddEntityFrameworkStores<PlanillaContext>()
.AddDefaultTokenProviders();

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
    options.IdleTimeout = TimeSpan.FromSeconds(10);
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
