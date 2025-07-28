using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;

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
            // Validation
            if (string.IsNullOrEmpty(toEmail))
                throw new ArgumentException("Email address cannot be null or empty", nameof(toEmail));
            
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentException("Subject cannot be null or empty", nameof(subject));
            
            if (string.IsNullOrEmpty(body))
                throw new ArgumentException("Body cannot be null or empty", nameof(body));

            // Kiểm tra cấu hình email
            var fromAddress = _configuration["EmailService:FromAddress"];
            var smtpHost = _configuration["EmailService:SmtpHost"];
            var smtpPort = _configuration["EmailService:SmtpPort"];
            var username = _configuration["EmailService:Username"];
            var password = _configuration["EmailService:Password"];

            if (string.IsNullOrEmpty(fromAddress) || string.IsNullOrEmpty(smtpHost) || 
                string.IsNullOrEmpty(smtpPort) || string.IsNullOrEmpty(username) || 
                string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("Email service configuration is incomplete. Please check appsettings.json");
            }

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(fromAddress));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(smtpHost, int.Parse(smtpPort), false);
                await smtp.AuthenticateAsync(username, password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
} 