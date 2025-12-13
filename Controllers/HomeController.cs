using LunchApp.Data;
using LunchApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    public HomeController(AppDbContext db) => _db = db;

    public IActionResult Index()
    {
        var username = HttpContext.Session.GetString("user");
        if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Auth");

        ViewBag.Username = username;
        return View();
    }

    [HttpPost]
    public IActionResult Signup(DateTime date)
    {
        var username = HttpContext.Session.GetString("user");
        if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Auth");

        if (date < DateTime.Today || (date == DateTime.Today && DateTime.Now.Hour >= 8))
        {
            ViewBag.Message = "Prijave so možne samo za prihodnje dni ali danes do 8:00!";
            return View("Index");
        }

        var user = _db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            ViewBag.Message = "Uporabnik ne obstaja!";
            return View("Index");
        }

        var record = _db.MealSignups.FirstOrDefault(s => s.UserId == user.Id && s.Date == date);
        if (record == null)
            _db.MealSignups.Add(new MealSignup { UserId = user.Id, Date = date, SignedUp = true });
        else
            record.SignedUp = true;

        _db.SaveChanges();
        ViewBag.Message = "Uspešno prijavljen!";
        return View("Index");
    }

    [HttpPost]
    public IActionResult Unsignup(DateTime date)
    {
        var username = HttpContext.Session.GetString("user");
        if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Auth");

        if (date < DateTime.Today || (date == DateTime.Today && DateTime.Now.Hour >= 8))
        {
            ViewBag.Message = "Odjave so možne samo za prihodnje dni ali danes do 8:00!";
            return View("Index");
        }

        var user = _db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            ViewBag.Message = "Uporabnik ne obstaja!";
            return View("Index");
        }

        var record = _db.MealSignups.FirstOrDefault(s => s.UserId == user.Id && s.Date == date);
        if (record == null)
            _db.MealSignups.Add(new MealSignup { UserId = user.Id, Date = date, SignedUp = false });
        else
            record.SignedUp = false;

        _db.SaveChanges();
        ViewBag.Message = "Uspešno odjavljen!";
        return View("Index");
    }

    public IActionResult MySignups()
    {
        var username = HttpContext.Session.GetString("user");
        if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Auth");

        var user = _db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null) return RedirectToAction("Login", "Auth");

        var signups = _db.MealSignups
            .Where(s => s.UserId == user.Id)
            .OrderBy(s => s.Date)
            .Include(s => s.User) // èe želiš uporabniške podatke v View
            .ToList();

        return View(signups);
    }
}
