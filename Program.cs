using CourierManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using CourierManagementSystem.Helpers;
using CourierManagementSystem.Models;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();


app.UseSession();
app.UseStaticFiles();
app.UseRouting();


// UPDATED: Landing page is now the Guest Tracking Portal
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Guest}/{action=Track}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();


    if (!db.Admins.Any())
    {
        db.Admins.Add(new Admin
        {
            Name = "Super Admin",
            Email = "admin@courierpro.com",
            Password = PasswordHelper.HashPassword("admin123"),
            PhoneNumber = "03001234567"
        });
        db.SaveChanges();
    }
}


app.Run();
