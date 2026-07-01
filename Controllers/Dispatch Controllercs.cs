using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourierManagementSystem.Data;
using CourierManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;


namespace CourierManagementSystem.Controllers
{
    public class DispatchController : Controller
    {
        private readonly AppDbContext _db;
        public DispatchController(AppDbContext db) => _db = db;


        // 1. SHOW UNASSIGNED PARCELS (Admin Only)
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");


            var unassignedParcels = _db.Parcels
                .Include(p => p.Sender)
                .Where(p => p.EmployeeId == null)
                .OrderByDescending(p => p.BookingDate)
                .ToList();


            return View(unassignedParcels);
        }


        // 2. ASSIGNMENT FORM (GET)
        [HttpGet]
        public IActionResult AssignDriver(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");


            var parcel = _db.Parcels.Include(p => p.Sender).FirstOrDefault(p => p.Id == id);
            if (parcel == null) return NotFound();


            // Fetch only active Drivers
            ViewBag.Drivers = _db.Employees
                .Where(e => e.Role.Equals("Driver") && e.IsActive == true)
                .ToList();


            return View(parcel);
        }


        // 3. SAVE ASSIGNMENT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignDriver(int parcelId, int driverId)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");


            var parcel = _db.Parcels.Find(parcelId);
            if (parcel != null)
            {
                parcel.EmployeeId = driverId;
                // Move status to InTransit automatically
                parcel.Status = ParcelStatus.InTransit;
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
