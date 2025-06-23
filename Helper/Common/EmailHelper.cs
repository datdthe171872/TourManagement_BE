using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace TourManagement_BE.Helper.Common
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;
        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailService:FromAddress"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration["EmailService:SmtpHost"], int.Parse(_configuration["EmailService:SmtpPort"]), false);
            await smtp.AuthenticateAsync(_configuration["EmailService:Username"], _configuration["EmailService:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
} 