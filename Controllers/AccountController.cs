using Microsoft.AspNetCore.Mvc;
using CourierManagementSystem.Data;
using CourierManagementSystem.Helpers;
using CourierManagementSystem.Models;
using System.Linq;


namespace CourierManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        public AccountController(AppDbContext db) => _db = db;


        [HttpGet]
        public IActionResult Login() => View();


        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // 1. ADMIN LOGIN 
            var admin = _db.Admins.FirstOrDefault(x => x.Email == email);
            if (admin != null && PasswordHelper.VerifyPassword(password, admin.Password))
            {
                HttpContext.Session.SetString("AdminName", admin.Name);
                HttpContext.Session.SetString("UserRole", "Admin");
                return RedirectToAction("Index", "Dashboard");
            }


            // 2. SECURE DRIVER LOGIN
            var employee = _db.Employees.FirstOrDefault(x => x.Email == email && x.Password == password);


            if (employee != null)
            {
                if (employee.Role.Equals("Driver", System.StringComparison.OrdinalIgnoreCase))
                {
                    HttpContext.Session.SetString("UserName", employee.Name);
                    // CRITICAL: This UserId is used to filter parcels
                    HttpContext.Session.SetString("UserId", employee.Id.ToString());
                    HttpContext.Session.SetString("UserRole", "Driver");
                    return RedirectToAction("Index", "Tracking");
                }
                else
                {
                    ViewBag.Error = "Access Denied: Only Delivery Drivers can log in here.";
                    return View();
                }
            }


            ViewBag.Error = "Invalid Email or Password!";
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Track", "Guest");
        }
    }
}


