using Microsoft.EntityFrameworkCore;
using LunchApp.Models;

namespace LunchApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MealSignup> MealSignups { get; set; }
    }
}
