using CourierManagementSystem.Data;
using CourierManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;


namespace CourierManagementSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _db;


        public CustomerController(AppDbContext db)
        {
            _db = db;
        }


        // ── 1. INDEX ─────────────────────────────────────────────────────────
        public IActionResult Index(string searchString, string trackingId)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");


            var customers = _db.Customers.AsQueryable();


            if (!string.IsNullOrEmpty(searchString))
                customers = customers.Where(c => c.Name.Contains(searchString)
                                              || c.Email.Contains(searchString));


            if (!string.IsNullOrEmpty(trackingId))
            {
                var parcel = _db.Parcels.FirstOrDefault(p => p.TrackingNumber == trackingId);
                customers = parcel != null
                    ? customers.Where(c => c.Id == parcel.CustomerId)
                    : customers.Where(c => false);
            }


            return View(customers.ToList());
        }


        // ── 2. CREATE GET ─────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");
            return View();
        }


        // ── 3. CREATE POST ────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer obj)
        {
            if (ModelState.IsValid)
            {
                _db.Customers.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }


        // ── 4. EDIT GET ───────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");


            var customer = _db.Customers.Find(id);
            if (customer == null) return NotFound();
            return View(customer);
        }


        // ── 5. EDIT POST ──────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer obj)
        {
            if (ModelState.IsValid)
            {
                _db.Customers.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }


        // ── 6. DETAILS ────────────────────────────────────────────────────────
        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");


            var customer = _db.Customers
                              .Include(c => c.BookedParcels)
                              .FirstOrDefault(c => c.Id == id);


            if (customer == null) return NotFound();
            return View(customer);
        }


        // ── 7. DELETE GET — shows confirmation page ───────────────────────────
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");


            // Load customer with parcel and invoice counts for the confirmation page
            var customer = _db.Customers
                              .Include(c => c.BookedParcels)
                                  .ThenInclude(p => p.Invoice)
                              .FirstOrDefault(c => c.Id == id);


            if (customer == null) return NotFound();
            return View(customer);
        }


        // ── 8. DELETE POST — cascade delete + log + transaction ───────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");


            // ── Begin transaction — all or nothing ────────────────────────────
            using var transaction = _db.Database.BeginTransaction();


            try
            {
                // Load customer with ALL related data
                var customer = _db.Customers
                                  .Include(c => c.BookedParcels)
                                      .ThenInclude(p => p.Invoice)
                                  .FirstOrDefault(c => c.Id == id);


                if (customer == null) return NotFound();


                int parcelsDeleted = 0;
                int invoicesDeleted = 0;


                // ── Step 1: delete invoices first (grandchildren) ─────────────
                foreach (var parcel in customer.BookedParcels.ToList())
                {
                    if (parcel.Invoice != null)
                    {
                        _db.Invoices.Remove(parcel.Invoice);
                        invoicesDeleted++;
                    }
                }


                // ── Step 2: delete parcels (children) ─────────────────────────
                foreach (var parcel in customer.BookedParcels.ToList())
                {
                    _db.Parcels.Remove(parcel);
                    parcelsDeleted++;
                }


                // ── Step 3: write to log table BEFORE deleting customer ────────
                _db.DeletedCustomerLogs.Add(new DeletedCustomerLog
                {
                    OriginalCustomerId = customer.Id,
                    Name = customer.Name,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.Address,
                    ParcelsDeleted = parcelsDeleted,
                    InvoicesDeleted = invoicesDeleted,
                    DeletedAt = DateTime.Now,
                    DeletedBy = "Admin"
                });


                // ── Step 4: delete the customer ───────────────────────────────
                _db.Customers.Remove(customer);


                // ── Step 5: one SaveChanges — commits everything together ──────
                _db.SaveChanges();


                // ── Step 6: commit transaction ────────────────────────────────
                transaction.Commit();


                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // ── Rollback — nothing gets deleted if anything fails ──────────
                transaction.Rollback();


                TempData["Error"] = $"Delete failed: {ex.Message}. No data was changed.";
                return RedirectToAction(nameof(Index));
            }
        }


        // ── 9. AJAX HELPERS ───────────────────────────────────────────────────

    }
}
