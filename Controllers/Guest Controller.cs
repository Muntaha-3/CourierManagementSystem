using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourierManagementSystem.Data;
using CourierManagementSystem.Models;


namespace CourierManagementSystem.Controllers
{
    public class GuestController : Controller
    {
        private readonly AppDbContext _db;
        public GuestController(AppDbContext db) => _db = db;


        // 1. Show the Search Page
        public IActionResult Track()
        {
            return View();
        }


        // 2. Process the Tracking ID
        [HttpPost]
        public IActionResult Track(string trackingId)
        {
            if (string.IsNullOrEmpty(trackingId))
            {
                ViewBag.Error = "Please enter a valid Tracking ID.";
                return View();
            }


            var parcel = _db.Parcels
                .Include(p => p.Sender)
                .FirstOrDefault(p => p.TrackingNumber == trackingId);


            if (parcel == null)
            {
                ViewBag.Error = "No record found for this Tracking ID. Please check and try again.";
                return View();
            }


            return View(parcel);
        }
    }
}
