using MimeKit; // MimeMessage, BodyBuilder
using MailKit.Net.Smtp; // MailKit SmtpClient
using MailKit.Security;

namespace LunchApp.Services
{
    public class EmailService
    {
        public void SendEmailWithAttachment(string toEmail, string subject, string body, string attachmentPath)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("LunchApp", "brigitapertovt@gmail.com"));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder { TextBody = body };

            if (!string.IsNullOrEmpty(attachmentPath))
                builder.Attachments.Add(attachmentPath);

            message.Body = builder.ToMessageBody();

            // uporabi MailKit SmtpClient s polnim imenom
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("tvoj.email@gmail.com", "tvoje_app_password"); // Gmail app password
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
