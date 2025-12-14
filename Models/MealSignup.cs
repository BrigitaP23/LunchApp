using System.ComponentModel.DataAnnotations;

namespace LunchApp.Models
{
    public class MealSignup
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool SignedUp { get; set; }

        // Relacija na uporabnika
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
