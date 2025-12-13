using LunchApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LunchApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Admin
        public IActionResult Index()
        {
            // Naložimo vse uporabnike z njihovimi prijavami/odjavami
            var users = _db.Users
                .Include(u => u.MealSignups)  // <-- popravljen property
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToList();

            return View(users);
        }
    }
}
