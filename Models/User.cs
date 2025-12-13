using System.Collections.Generic;

namespace LunchApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        // Dodaj navigacijski property
        public List<MealSignup> MealSignups { get; set; } = new List<MealSignup>();
    }
}
