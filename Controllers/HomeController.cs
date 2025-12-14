using LunchApp.Data;
using LunchApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LunchApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("user");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Auth");

            ViewBag.Username = username;
            return View();
        }

        [HttpPost]
        public IActionResult Signup(DateTime date)
        {
            var username = HttpContext.Session.GetString("user");

            if (date < DateTime.Today || (date == DateTime.Today && DateTime.Now.Hour >= 8))
            {
                ViewBag.Message = "Prijave so možne samo za prihodnje dni ali danes do 8:00!";
                return View("Index");
            }

            var user = _db.User.Include(u => u.MealSignups).FirstOrDefault(u => u.Username == username);
            if (user == null) return RedirectToAction("Login", "Auth");

            var signup = user.MealSignups.FirstOrDefault(s => s.Date == date);
            if (signup == null)
                user.MealSignups.Add(new MealSignup { Date = date, SignedUp = true });
            else
                signup.SignedUp = true;

            _db.SaveChanges();

            ViewBag.Message = "Uspešno prijavljen!";
            return View("Index");
        }

        [HttpPost]
        public IActionResult Unsignup(DateTime date)
        {
            var username = HttpContext.Session.GetString("user");

            if (date < DateTime.Today || (date == DateTime.Today && DateTime.Now.Hour >= 8))
            {
                ViewBag.Message = "Odjave so možne samo za prihodnje dni ali danes do 8:00!";
                return View("Index");
            }

            var user = _db.User.Include(u => u.MealSignups).FirstOrDefault(u => u.Username == username);
            if (user == null) return RedirectToAction("Login", "Auth");

            var signup = user.MealSignups.FirstOrDefault(s => s.Date == date);
            if (signup == null)
                user.MealSignups.Add(new MealSignup { Date = date, SignedUp = false });
            else
                signup.SignedUp = false;

            _db.SaveChanges();

            ViewBag.Message = "Uspešno odjavljen!";
            return View("Index");
        }

        public IActionResult MySignups()
        {
            var username = HttpContext.Session.GetString("user");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Auth");

            var user = _db.User.Include(u => u.MealSignups)
                               .FirstOrDefault(u => u.Username == username);

            return View(user?.MealSignups.OrderBy(s => s.Date).ToList());
        }
    }
}
