using LCDE;
using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IRepositorioUsuarios, RepositorioUsuarios>();
builder.Services.AddTransient<IUserStore<Usuario>, UsuarioStore>();
builder.Services.AddTransient<RepositotioClientes, RepositotioClientes>();
builder.Services.AddTransient<RepositorioProveedores, RepositorioProveedores>();
builder.Services.AddTransient<RepositorioProductos, RepositorioProductos>();
builder.Services.AddTransient<RepositorioCategorias, RepositorioCategorias>();
builder.Services.AddTransient<RepositorioTipoPago, RepositorioTipoPago>();
builder.Services.AddTransient<RepositorioVentas, RepositorioVentas>();
builder.Services.AddTransient<ReportesServicio, ReportesServicio>();
builder.Services.AddTransient<RepositorioDevoluciones, RepositorioDevoluciones>();
builder.Services.AddTransient<RepositorioRegistroCaja, RepositorioRegistroCaja>();
builder.Services.AddTransient<RepositorioReportes, RepositorioReportes>();

builder.Services.AddTransient<RepositorioPromociones, RepositorioPromociones>();



builder.Services.AddTransient<SignInManager<Usuario>>();
builder.Services.AddIdentityCore<Usuario>(opciones =>
{
    opciones.Password.RequireDigit = false;
    opciones.Password.RequireLowercase = false;
    opciones.Password.RequireUppercase = false;
    opciones.Password.RequireNonAlphanumeric = false;
}).AddErrorDescriber<MensajesDeErrorIdentity>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, opciones =>
{
    opciones.LoginPath = "/Auth/login";
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
    pattern: "{controller=Ventas}/{action=Index}/{id?}");

app.Run();
