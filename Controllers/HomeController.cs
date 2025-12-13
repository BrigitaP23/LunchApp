using LunchApp.Data;
using LunchApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    public HomeController(AppDbContext db) => _db = db;

    public IActionResult Index()
    {
        var user = HttpContext.Session.GetString("user");
        if (string.IsNullOrEmpty(user)) return RedirectToAction("Login", "Auth");

        ViewBag.Username = user;
        return View();
    }

    [HttpPost]
    public IActionResult Signup(DateTime date)
    {
        string user = HttpContext.Session.GetString("user");
        if (date < DateTime.Today || (date == DateTime.Today && DateTime.Now.Hour >= 8))
        {
            ViewBag.Message = "Prijave so možne samo za prihodnje dni ali danes do 8:00!";
            return View("Index");
        }

        var record = _db.MealSignups.FirstOrDefault(s => s.Username == user && s.Date == date);
        if (record == null) _db.MealSignups.Add(new MealSignup { Username = user, Date = date, SignedUp = true });
        else record.SignedUp = true;
        _db.SaveChanges();

        ViewBag.Message = "Uspešno prijavljen!";
        return View("Index");
    }

    [HttpPost]
    public IActionResult Unsignup(DateTime date)
    {
        string user = HttpContext.Session.GetString("user");
        if (date < DateTime.Today || (date == DateTime.Today && DateTime.Now.Hour >= 8))
        {
            ViewBag.Message = "Odjave so možne samo za prihodnje dni ali danes do 8:00!";
            return View("Index");
        }

        var record = _db.MealSignups.FirstOrDefault(s => s.Username == user && s.Date == date);
        if (record == null) _db.MealSignups.Add(new MealSignup { Username = user, Date = date, SignedUp = false });
        else record.SignedUp = false;
        _db.SaveChanges();

        ViewBag.Message = "Uspešno odjavljen!";
        return View("Index");
    }

    public IActionResult MySignups()
    {
        string user = HttpContext.Session.GetString("user");
        if (string.IsNullOrEmpty(user)) return RedirectToAction("Login", "Auth");

        var signups = _db.MealSignups.Where(s => s.Username == user).OrderBy(s => s.Date).ToList();
        return View(signups);
    }
}
