using LunchApp.Data;
using LunchApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LunchApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Auth/Login
        public IActionResult Login() => View();

        // POST: /Auth/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vsa polja so obvezna!";
                return View();
            }

            var user = _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                ViewBag.Error = "Napačno uporabniško ime ali geslo";
                return View();
            }

            // Shrani uporabniško ime v session
            HttpContext.Session.SetString("user", username);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Auth/Register
        public IActionResult Register() => View();

        // POST: /Auth/Register
        [HttpPost]
        public IActionResult Register(string firstName, string lastName, string password)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vsa polja so obvezna!";
                return View();
            }

            // Sestavi uporabniško ime: ime.priimek
            string username = $"{firstName.Trim().ToLower()}.{lastName.Trim().ToLower()}";

            // Preveri, če uporabnik že obstaja
            if (_db.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Uporabnik s tem imenom že obstaja!";
                return View();
            }

            // Ustvari novega uporabnika
            var user = new User
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Username = username,
                Password = password
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            // Po registraciji uporabnika preusmeri na Login (ni samodejne prijave)
            return RedirectToAction("Login");
        }

        // Odjava
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            return RedirectToAction("Login");
        }
    }
}
