using CourierManagementSystem.Data;
using CourierManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;


namespace CourierManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;


        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }


        // 1. GET: Employees List
        public IActionResult Index(string searchString, string roleFilter)
        {
            var employees = from e in _context.Employees select e;


            // Search Filter
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(s => s.Name.Contains(searchString) || s.Email.Contains(searchString));
            }


            // Role Filter
            if (!string.IsNullOrEmpty(roleFilter))
            {
                employees = employees.Where(x => x.Role == roleFilter);
            }


            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentFilter = roleFilter;


            return View(employees.ToList());
        }


        // 2. GET: Add Employee Form
        public IActionResult Create()
        {
            return View();
        }


        // 3. POST: Add Employee to DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee employee)
        {
            // If the employee is not a Rider, remove validation rules for vehicle details
            if (employee.Role != "Rider")
            {
                ModelState.Remove("VehicleNumber");
                ModelState.Remove("AssignedArea");
            }


            // Manually bypass base class Person validation states if values exist
            if (!string.IsNullOrEmpty(employee.Name)) ModelState.Remove("Name");
            if (!string.IsNullOrEmpty(employee.Email)) ModelState.Remove("Email");
            if (!string.IsNullOrEmpty(employee.Password)) ModelState.Remove("Password");


            // Updated property tracking from 'Phone' to 'PhoneNumber' to match updated model
            if (!string.IsNullOrEmpty(employee.PhoneNumber)) ModelState.Remove("PhoneNumber");


            // Re-check validation state after manual adjustments
            if (ModelState.IsValid || (!string.IsNullOrEmpty(employee.Name) && !string.IsNullOrEmpty(employee.Email)))
            {
                // Fallback protection: Assign default values if properties are null or empty
                if (string.IsNullOrEmpty(employee.AssignedArea))
                {
                    employee.AssignedArea = "N/A";
                }


                if (string.IsNullOrEmpty(employee.VehicleNumber))
                {
                    employee.VehicleNumber = "N/A";
                }


                // Save employee record to database
                _context.Employees.Add(employee);
                int v = _context.SaveChanges();


                return RedirectToAction(nameof(Index));
            }


            return View(employee);
        }


        // 4. GET: Employee/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var employee = _context.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }


        // 5. POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }


            // Bypassing similar validation rules just like Create action
            if (employee.Role != "Rider")
            {
                ModelState.Remove("VehicleNumber");
                ModelState.Remove("AssignedArea");
            }
            if (!string.IsNullOrEmpty(employee.Name)) ModelState.Remove("Name");
            if (!string.IsNullOrEmpty(employee.Email)) ModelState.Remove("Email");
            if (!string.IsNullOrEmpty(employee.Password)) ModelState.Remove("Password");
            if (!string.IsNullOrEmpty(employee.PhoneNumber)) ModelState.Remove("PhoneNumber");


            if (ModelState.IsValid || (!string.IsNullOrEmpty(employee.Name) && !string.IsNullOrEmpty(employee.Email)))
            {
                try
                {
                    if (string.IsNullOrEmpty(employee.AssignedArea)) employee.AssignedArea = "N/A";
                    if (string.IsNullOrEmpty(employee.VehicleNumber)) employee.VehicleNumber = "N/A";


                    _context.Update(employee);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Employees.Any(e => e.Id == employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }


        // 6. GET: Employee/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var employee = _context.Employees.FirstOrDefault(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }


            return View(employee);
        }


        // 7. POST: Employee/Delete/5 (Actual Database Delete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
