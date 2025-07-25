using DA.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace DA.SendEmail
{
    public class MailHelper
    {
        private readonly EmailSettings _settings;

        public MailHelper(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendMail(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSSL,
                Credentials = new NetworkCredential(_settings.From, _settings.Password)
            })
            using (var message = new MailMessage(_settings.From, toEmail, subject, body))
            {
                message.IsBodyHtml = false; 
                await client.SendMailAsync(message);
            }
        }
    }

   
}
