using LunchApp.Data;
using LunchApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LunchApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        public AuthController(AppDbContext db) => _db = db;

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                ViewBag.Error = "Napačno uporabniško ime ali geslo";
                return View();
            }

            HttpContext.Session.SetString("user", username);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(string firstName, string lastName, string password)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vsa polja so obvezna!";
                return View();
            }

            string username = $"{firstName.ToLower()}.{lastName.ToLower()}";

            if (_db.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Uporabnik s tem imenom že obstaja!";
                return View();
            }

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                Password = password
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            HttpContext.Session.SetString("user", username);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            return RedirectToAction("Login");
        }
    }
}
