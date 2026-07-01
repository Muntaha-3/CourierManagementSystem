using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourierManagementSystem.Data;
using CourierManagementSystem.Models;
using System.Linq;


namespace CourierManagementSystem.Controllers
{
    public class TrackingController : Controller
    {
        private readonly AppDbContext _db;
        public TrackingController(AppDbContext db) => _db = db;


        // 1. TRACKING HUB (Filtered by logged-in Driver ID)
        public IActionResult Index(string searchId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userIdStr = HttpContext.Session.GetString("UserId");


            if (string.IsNullOrEmpty(role)) return RedirectToAction("Login", "Account");


            // Start query and include Sender for the Name display
            var query = _db.Parcels.Include(p => p.Sender).AsQueryable();


            // --- THE PRIVACY FILTER ---
            if (role == "Driver")
            {
                int loggedInDriverId = int.Parse(userIdStr);
                // Filter: Only show parcels assigned to this specific driver
                query = query.Where(p => p.EmployeeId == loggedInDriverId);
            }


            // Apply Search filter if typed
            if (!string.IsNullOrEmpty(searchId))
            {
                query = query.Where(p => p.TrackingNumber.Contains(searchId));
            }


            return View(query.OrderByDescending(p => p.BookingDate).ToList());
        }


        // 2. UPDATE STATUS (GET)
        public IActionResult UpdateStatus(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userIdStr = HttpContext.Session.GetString("UserId");


            var parcel = _db.Parcels.Find(id);
            if (parcel == null) return NotFound();


            // SECURITY CHECK: Prevent Driver A from opening Driver B's parcel via URL
            if (role == "Driver")
            {
                int loggedInDriverId = int.Parse(userIdStr);
                if (parcel.EmployeeId != loggedInDriverId)
                {
                    return RedirectToAction("Index"); // Kick them back to their own list
                }
            }


            return View(parcel);
        }


        // 3. SAVE STATUS (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, ParcelStatus status, string currentLocation)
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userIdStr = HttpContext.Session.GetString("UserId");


            var parcel = _db.Parcels.Find(id);
            if (parcel != null)
            {
                // FINAL SECURITY CHECK: Ensure this driver is actually assigned to this parcel
                if (role == "Driver" && parcel.EmployeeId != int.Parse(userIdStr))
                {
                    return Unauthorized();
                }


                parcel.Status = status;
                parcel.CurrentLocation = currentLocation;
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
