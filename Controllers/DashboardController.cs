using Microsoft.AspNetCore.Mvc;


public class DashboardController : Controller
{
    public IActionResult Index()
    {
        // SECURITY GATEKEEPER
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToAction("Login", "Account");


        return View();
    }
}
