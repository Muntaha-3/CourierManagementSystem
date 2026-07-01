using CourierManagementSystem.Data;
using CourierManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CourierManagementSystem.Controllers
{
    public class ParcelController : Controller
    {
        private readonly AppDbContext _db;
        public ParcelController(AppDbContext db) => _db = db;


        // ── INDEX ─────────────────────────────────────────────────────────────
        public IActionResult Index()
        {
            var parcels = _db.Parcels
                             .Include(p => p.Sender)
                             .OrderByDescending(p => p.BookingDate)
                             .ToList();
            return View(parcels);
        }


        // ── CREATE GET ────────────────────────────────────────────────────────
        public IActionResult Create()
        {
            ViewBag.Customers = _db.Customers.ToList();
            return View();
        }


        // ── CREATE POST ───────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Parcel obj)
        {
            ModelState.Remove("Sender");
            ModelState.Remove("AssignedDriver");
            ModelState.Remove("Invoice");
            ModelState.Remove("TrackingNumber");


            if (ModelState.IsValid)
            {
                decimal basePrice = (obj.Service == ServiceType.Express) ? 450 : 250;
                decimal weightCharge = (decimal)obj.Weight * 150;
                obj.TotalCost = basePrice + weightCharge;


                _db.Parcels.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.Customers = _db.Customers.ToList();
            return View(obj);
        }


        // ── EDIT GET ──────────────────────────────────────────────────────────
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var parcel = _db.Parcels.Find(id);
            if (parcel == null) return NotFound();


            ViewBag.Customers = _db.Customers.ToList();
            return View(parcel);
        }


        // ── EDIT POST ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Parcel obj)
        {
            if (id != obj.Id) return NotFound();


            ModelState.Remove("Sender");
            ModelState.Remove("AssignedDriver");
            ModelState.Remove("Invoice");
            ModelState.Remove("TrackingNumber");


            if (ModelState.IsValid)
            {
                decimal basePrice = (obj.Service == ServiceType.Express) ? 450 : 250;
                decimal weightCharge = (decimal)obj.Weight * 150;
                obj.TotalCost = basePrice + weightCharge;


                _db.Parcels.Update(obj);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }


            ViewBag.Customers = _db.Customers.ToList();
            return View(obj);
        }


        // ── DELETE GET — confirmation page ────────────────────────────────────
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();


            // Include Invoice so confirmation page can warn user
            var parcel = _db.Parcels
                            .Include(p => p.Sender)
                            .Include(p => p.Invoice)
                            .FirstOrDefault(p => p.Id == id);


            if (parcel == null) return NotFound();
            return View(parcel);
        }


        // ── DELETE POST — cascade delete + transaction ────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // ── Begin transaction — all or nothing ────────────────────────────
            using var transaction = _db.Database.BeginTransaction();


            try
            {
                // Load parcel WITH its invoice (child record)
                var parcel = _db.Parcels
                                .Include(p => p.Invoice)
                                .FirstOrDefault(p => p.Id == id);


                if (parcel == null) return NotFound();


                // ── Step 1: delete invoice first (child of parcel) ────────────
                if (parcel.Invoice != null)
                    _db.Invoices.Remove(parcel.Invoice);


                // ── Step 2: delete parcel ─────────────────────────────────────
                _db.Parcels.Remove(parcel);


                // ── Step 3: one SaveChanges — both deletions in one shot ───────
                _db.SaveChanges();


                // ── Step 4: commit ────────────────────────────────────────────
                transaction.Commit();


                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // ── Rollback — nothing deleted if anything fails ───────────────
                transaction.Rollback();


                TempData["Error"] = $"Delete failed: {ex.Message}. No data was changed.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
