using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourierManagementSystem.Data;
using CourierManagementSystem.Models;
using System.Linq;


namespace CourierManagementSystem.Controllers
{
    public class DriverController : Controller
    {
        private readonly AppDbContext _db;
        public DriverController(AppDbContext db) => _db = db;


        // THE PRIVATE TASK LIST
        public IActionResult MyTasks()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userIdStr = HttpContext.Session.GetString("UserId");


            if (role != "Driver" || string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }


            int driverId = int.Parse(userIdStr);


            // Fetch only parcels assigned to this Driver
            var parcels = _db.Parcels
                .Where(p => p.EmployeeId == driverId)
                .OrderByDescending(p => p.BookingDate)
                .ToList();


            return View(parcels);
        }
    }
}
