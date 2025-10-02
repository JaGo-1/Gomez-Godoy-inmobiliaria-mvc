using System.Security.Claims;
using inmobiliaria_mvc.Repository;
using inmobiliaria_mvc.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuarios/Login";
        options.LogoutPath = "/Usuarios/Logout";
        options.AccessDeniedPath = "/Home/Restringido";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Administrador")
    );
    options.AddPolicy("SuperAdministrador", policy =>
        policy.RequireClaim(ClaimTypes.Role, "SuperAdministrador")
    );
    options.AddPolicy("Empleado", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Empleado")
    );
});

builder.Services.AddTransient<IRepositoryInquilino, RepositoryInquilino>();
builder.Services.AddTransient<IRepositoryPropietario, RepositoryPropietario>();
builder.Services.AddTransient<IRepositoryInmueble, RepositoryInmueble>();
builder.Services.AddTransient<IRepositoryContrato, RepositoryContrato>();
builder.Services.AddTransient<IRepositoryPago, RepositoryPago>();
builder.Services.AddTransient<IRepositoryImagen, RepositoryImagen>();
builder.Services.AddTransient<IRepositoryUsuario, RepositoryUsuario>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IRepositoryAuditoria, RepositoryAuditoria>();



var app = builder.Build();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();