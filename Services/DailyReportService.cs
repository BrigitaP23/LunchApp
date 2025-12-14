using Microsoft.Extensions.Hosting;
using LunchApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LunchApp.Services
{
    public class DailyReportService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private DateTime _lastSent = DateTime.MinValue;

        public DailyReportService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                if (now.Hour == 8 && now.Minute == 1 && _lastSent.Date != now.Date)
                {
                    await SendReport();
                    _lastSent = now;
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task SendReport()
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var email = scope.ServiceProvider.GetRequiredService<EmailService>();

            var today = DateTime.Today;

            var data = await db.MealSignup
     .Include(ms => ms.User) // 🔹 naloži uporabnika
     .Where(x => x.Date == today)
     .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Ime;Priimek;Uporabniško ime;Status");

            foreach (var s in data)
            {
                var user = s.User; // 🔹 dostop do uporabnika preko navigacijske lastnosti
                sb.AppendLine($"{user.FirstName};{user.LastName};{user.Username};{(s.SignedUp ? "PRIJAVLJEN" : "ODJAVLJEN")}");
            }

            var path = "/app/daily_report.csv";
            File.WriteAllText(path, sb.ToString());

            email.Send(
                "Kosila – poročilo za danes",
                $"Poročilo za {today:dd.MM.yyyy}",
                path
            );
        }
    }
}
