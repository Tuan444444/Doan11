using DA.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace DA.SendEmail
{
    public class MailHelper
    {
        private readonly EmailSettings _settings;

        public MailHelper(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendMailAsync(string to, string subject, string body)
        {
            var fromAddress = new MailAddress(_settings.From, _settings.DisplayName);
            var toAddress = new MailAddress(to);

            using var smtp = new SmtpClient
            {
                Host = _settings.Host,
                Port = _settings.Port,
                EnableSsl = _settings.EnableSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, _settings.Password)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}
