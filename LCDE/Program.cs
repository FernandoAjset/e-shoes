using LCDE;
using LCDE.Models;
using LCDE.Models.Enums;
using LCDE.Servicios;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IRepositorioUsuarios, RepositorioUsuarios>();
builder.Services.AddTransient<IUserStore<Usuario>, UsuarioStore>();
builder.Services.AddTransient<IRepositorioCliente, RepositotioClientes>();
builder.Services.AddTransient<RepositorioProveedores, RepositorioProveedores>();
builder.Services.AddTransient<RepositorioProductos, RepositorioProductos>();
builder.Services.AddTransient<RepositorioCategorias, RepositorioCategorias>();
builder.Services.AddTransient<RepositorioTipoPago, RepositorioTipoPago>();
builder.Services.AddTransient<RepositorioVentas, RepositorioVentas>();
builder.Services.AddTransient<ReportesServicio, ReportesServicio>();
builder.Services.AddTransient<RepositorioDevoluciones, RepositorioDevoluciones>();
builder.Services.AddTransient<RepositorioRegistroCaja, RepositorioRegistroCaja>();
builder.Services.AddTransient<RepositorioReportes, RepositorioReportes>();
builder.Services.AddSingleton<IEncryptService, EncryptService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IRepositorioToken, RepositorioToken>();
builder.Services.AddTransient<ILogService, LogService>();

builder.Services.AddTransient<RepositorioPromociones, RepositorioPromociones>();
builder.Services.AddTransient<SignInManager<Usuario>>();
builder.Services.AddIdentityCore<Usuario>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
}).AddErrorDescriber<MensajesDeErrorIdentity>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, options =>
{
    options.AccessDeniedPath = "/Auth/Unauthorized";
    options.LoginPath = "/Auth/login";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole(((int)Rol.Admin).ToString()));
    options.AddPolicy("VendedorPolicy", policy => policy.RequireRole(((int)Rol.Vendedor).ToString()));
    options.AddPolicy("ClientePolicy", policy => policy.RequireRole(((int)Rol.Cliente).ToString()));
});

builder.Services.AddTransient<ISesionServicio, SesionServicio>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
    pattern: "{controller=Ventas}/{action=Index}/{id?}");

app.Run();