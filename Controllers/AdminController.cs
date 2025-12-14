using LunchApp.Data;
using LunchApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            // 🔹 Naložimo vse uporabnike skupaj z njihovimi MealSignups
            var users = _db.User
                .Include(u => u.MealSignups)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToList();

            return View(users);
        }
    }
}
