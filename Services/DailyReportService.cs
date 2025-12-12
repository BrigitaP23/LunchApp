using LunchApp.Models;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace LunchApp.Services
{
    public class DailyReportService : BackgroundService
    {
        private readonly FileService fileService;
        private readonly EmailService emailService;

        public DailyReportService(FileService fileService, EmailService emailService)
        {
            this.fileService = fileService;
            this.emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = new DateTime(now.Year, now.Month, now.Day, 8, 1, 0);
                if (now > nextRun)
                    nextRun = nextRun.AddDays(1);

                var delay = nextRun - now;
                await Task.Delay(delay, stoppingToken);

                SendDailyReport();
            }
        }

        private void SendDailyReport()
        {
            var today = DateTime.Today;
            var signups = fileService.LoadSignups()
                .Where(s => s.Date == today)
                .ToList();

            var filePath = Path.Combine("data", $"LunchReport_{today:yyyy-MM-dd}.csv");
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Ime,Priimek,Username,Status");
                foreach (var s in signups)
                {
                    var user = fileService.LoadUsers().FirstOrDefault(u => u.Username == s.Username);
                    if (user != null)
                    {
                        string status = s.SignedUp ? "Prijavljen" : "Odjavljen";
                        writer.WriteLine($"{user.FirstName},{user.LastName},{user.Username},{status}");
                    }
                }
            }

            emailService.SendEmailWithAttachment(
                "brigitapertovt@gmail.com",
                $"Dnevni report kosil {today:yyyy-MM-dd}",
                "V priponki je seznam prijav in odjav za današnji dan.",
                filePath
            );
        }
    }
}
