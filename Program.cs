using inmobiliaria_mvc.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IRepositoryInquilino, RepositoryInquilino>();
builder.Services.AddTransient<IRepositoryPropietario, RepositoryPropietario>();
builder.Services.AddTransient<IRepositoryInmueble, RepositoryInmueble>();
builder.Services.AddTransient<IRepositoryContrato, RepositoryContrato>();
builder.Services.AddTransient<IRepositoryPago, RepositoryPago>();


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