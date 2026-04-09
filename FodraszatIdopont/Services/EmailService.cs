using System.Net;
using System.Net.Mail;
using FodraszatIdopont.Services.Interface;
using Microsoft.Extensions.Configuration;

namespace FodraszatIdopont.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            string senderEmail = _config["EmailSettings:SenderEmail"]!;
            string senderPassword = _config["EmailSettings:SenderPassword"]!;

            using var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, "Wild Cut Fodrászat"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}