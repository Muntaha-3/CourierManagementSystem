using CourierManagementSystem.Data;
using CourierManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace CourierManagementSystem.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly AppDbContext _db;
        public InvoiceController(AppDbContext db) => _db = db;


        // ── 1. INDEX: LIST ALL INVOICES ──────────────────────────────
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");


            var invoices = _db.Invoices
                .Include(i => i.Parcel)
                .Include(i => i.Customer)
                .OrderByDescending(i => i.IssueDate)
                .ToList();


            return View(invoices);
        }


        // ── 2. DETAILS: VIEW A SPECIFIC INVOICE ──────────────────────
        public IActionResult Details(int? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");


            if (id == null) return NotFound();


            var invoice = _db.Invoices
                .Include(i => i.Parcel)
                .Include(i => i.Customer)
                .FirstOrDefault(m => m.Id == id);


            if (invoice == null) return NotFound();


            return View(invoice);
        }


        // ── 3. CREATE (GET) ───────────────────────────────────────────
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");


            ViewBag.Customers = _db.Customers.ToList();


            // Only show parcels that don't have an invoice yet
            var invoicedParcelIds = _db.Invoices.Select(i => i.ParcelId).ToHashSet();
            ViewBag.Parcels = _db.Parcels
                .Include(p => p.Sender)
                .Where(p => !invoicedParcelIds.Contains(p.Id))
                .ToList();


            return View();
        }


        // ── 4. CREATE (POST) ──────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Invoice obj)
        {
            ModelState.Clear(); // Force clean start to avoid "Invalid Value" errors


            var parcel = _db.Parcels.Find(obj.ParcelId);
            if (parcel == null) return NotFound();


            // PKR Logic: If Cash, ignore the Bill To selection and use the Parcel's Sender
            if (obj.PaymentMethod == PaymentMethod.Cash)
            {
                obj.CustomerId = parcel.CustomerId;
            }


            obj.ParcelBaseAmount = parcel.TotalCost;
            CalculateTotals(obj); // Perform PKR Math


            _db.Invoices.Add(obj);
            _db.SaveChanges();


            return RedirectToAction(nameof(Index));
        }


        // ── 5. EDIT (GET) ──────────────────────────────────────────────
        public IActionResult Edit(int? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();


            var invoice = _db.Invoices.Find(id);
            if (invoice == null) return NotFound();


            ViewBag.Customers = _db.Customers.ToList();
            ViewBag.Parcels = _db.Parcels.Include(p => p.Sender).ToList();
            return View(invoice);
        }


        // ── 6. EDIT (POST) ─────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Invoice obj)
        {
            ModelState.Clear();
            if (id != obj.Id) return NotFound();


            var parcel = _db.Parcels.Find(obj.ParcelId);
            if (parcel != null)
            {
                obj.ParcelBaseAmount = parcel.TotalCost;
                CalculateTotals(obj);
                _db.Invoices.Update(obj);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }


        // ── 7. DELETE (GET) ────────────────────────────────────────────
        public IActionResult Delete(int? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();


            var invoice = _db.Invoices.Include(i => i.Parcel).Include(i => i.Customer)
                            .FirstOrDefault(m => m.Id == id);
            if (invoice == null) return NotFound();


            return View(invoice);
        }


        // ── 8. DELETE (POST) ──────────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var invoice = _db.Invoices.Find(id);
            if (invoice != null)
            {
                _db.Invoices.Remove(invoice);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }


        // ── AJAX HELPER: FETCH COST & SENDER ID ──────────────────────
        [HttpGet]
        public IActionResult GetParcelCost(int parcelId)
        {
            var parcel = _db.Parcels.Find(parcelId);
            // We return both cost AND the correct CustomerId (Sender)
            return Json(new
            {
                cost = parcel?.TotalCost ?? 0,
                senderId = parcel?.CustomerId ?? 0
            });
        }


        // ── PRIVATE MATH ENGINE ──────────────────────────────────────
        private static void CalculateTotals(Invoice inv)
        {
            inv.SubTotal = inv.ParcelBaseAmount + inv.FuelSurcharge + inv.RemoteAreaSurcharge + inv.InsuranceFee;
            inv.GstAmount = inv.SubTotal * (decimal)(inv.GstPercent / 100.0);
            inv.TotalTax = inv.GstAmount;
            inv.TotalDiscount = (inv.SubTotal * (decimal)(inv.DiscountPercent / 100.0)) + inv.GiftCodeDiscount;
            inv.GrandTotal = inv.SubTotal + inv.TotalTax - inv.TotalDiscount;
            inv.BalanceDue = inv.GrandTotal - inv.AmountPaid;
            if (inv.AmountPaid >= inv.GrandTotal && inv.GrandTotal > 0) inv.Status = InvoiceStatus.Paid;
        }
    }
}
