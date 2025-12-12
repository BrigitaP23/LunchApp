using Microsoft.AspNetCore.Mvc;
using LunchApp.Services;
using LunchApp.Models;

namespace LunchApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly FileService fileService = new();

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
            if (string.IsNullOrEmpty(user)) return RedirectToAction("Login", "Auth");

            if (date < DateTime.Today || (date == DateTime.Today && DateTime.Now.Hour >= 8))
            {
                ViewBag.Message = "Prijave so mogoèe samo za današnji dan do 8:00 ali za prihodnje dni.";
                ViewBag.Username = user;
                return View("Index");
            }

            var signups = fileService.LoadSignups();
            var record = signups.FirstOrDefault(s => s.Username == user && s.Date == date);
            if (record == null) signups.Add(new MealSignup { Username = user, Date = date, SignedUp = true });
            else record.SignedUp = true;
            fileService.SaveSignups(signups);

            ViewBag.Message = "Uspešno prijavljen!";
            ViewBag.Username = user;
            return View("Index");
        }

        [HttpPost]
        public IActionResult Unsignup(DateTime date)
        {
            string user = HttpContext.Session.GetString("user");
            if (string.IsNullOrEmpty(user)) return RedirectToAction("Login", "Auth");

            if (date < DateTime.Today || (date == DateTime.Today && DateTime.Now.Hour >= 8))
            {
                ViewBag.Message = "Odjave so mogoèe samo za današnji dan do 8:00 ali za prihodnje dni.";
                ViewBag.Username = user;
                return View("Index");
            }

            var signups = fileService.LoadSignups();
            var record = signups.FirstOrDefault(s => s.Username == user && s.Date == date);
            if (record == null) signups.Add(new MealSignup { Username = user, Date = date, SignedUp = false });
            else record.SignedUp = false;
            fileService.SaveSignups(signups);

            ViewBag.Message = "Uspešno odjavljen!";
            ViewBag.Username = user;
            return View("Index");
        }

        public IActionResult MySignups()
        {
            string user = HttpContext.Session.GetString("user");
            if (string.IsNullOrEmpty(user)) return RedirectToAction("Login", "Auth");

            var signups = fileService.LoadSignups()
                .Where(s => s.Username == user)
                .OrderBy(s => s.Date)
                .ToList();

            return View(signups);
        }
    }
}
