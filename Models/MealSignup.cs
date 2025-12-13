namespace LunchApp.Models
{
    public class MealSignup
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public bool SignedUp { get; set; }
    }
}
