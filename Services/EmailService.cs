using System.Net;
using System.Net.Mail;

namespace LunchApp.Services
{
    public class EmailService
    {
        public void Send(string subject, string body, string attachmentPath)
        {
            var message = new MailMessage();
            message.From = new MailAddress("brigitapertovt@gmail.com");
            message.To.Add("brigitapertovt@gmail.com");
            message.Subject = subject;
            message.Body = body;

            if (!string.IsNullOrEmpty(attachmentPath))
                message.Attachments.Add(new Attachment(attachmentPath));

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(
                    Environment.GetEnvironmentVariable("EMAIL_USER"),
                    Environment.GetEnvironmentVariable("EMAIL_PASS")
                ),
                EnableSsl = true
            };

            smtp.Send(message);
        }
    }
}
