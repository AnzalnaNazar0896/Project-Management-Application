using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectTracker.Core.Interfaces;

namespace ProjectTracker.Infrastructure.Email
{
    public class SmtpEmailNotificationService : IEmailNotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailNotificationService> _logger;

        public SmtpEmailNotificationService(
            IConfiguration configuration,
            ILogger<SmtpEmailNotificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool IsEnabled =>
            _configuration.GetValue<bool>("Email:Enabled");

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            if (!IsEnabled || string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogInformation("Email skipped (disabled or empty recipient): {Subject} -> {To}", subject, toEmail);
                return;
            }

            var host = _configuration["Email:SmtpHost"];
            var port = _configuration.GetValue<int>("Email:SmtpPort", 587);
            var user = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];
            var from = _configuration["Email:From"] ?? user;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
            {
                _logger.LogWarning("Email not sent: SMTP host or From address is not configured.");
                return;
            }

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = _configuration.GetValue<bool>("Email:UseSsl", true),
                Credentials = string.IsNullOrEmpty(user)
                    ? CredentialCache.DefaultNetworkCredentials
                    : new NetworkCredential(user, password)
            };

            using var message = new MailMessage(from, toEmail, subject, htmlBody) { IsBodyHtml = true };
            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent: {Subject} -> {To}", subject, toEmail);
        }
    }
}
