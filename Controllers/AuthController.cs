using Microsoft.AspNetCore.Mvc;
using LunchApp.Services;
using LunchApp.Models;

namespace LunchApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly FileService fileService = new();

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var users = fileService.LoadUsers();
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

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

            var users = fileService.LoadUsers();
            if (users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Uporabnik s tem imenom že obstaja!";
                return View();
            }

            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                Password = password
            };

            users.Add(newUser);
            fileService.SaveUsers(users);

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            return RedirectToAction("Login");
        }
    }
}
