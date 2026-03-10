using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace ParlorBookingSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();

            // Added '!' to silence the null warnings
            email.From.Add(new MailboxAddress(
                _config["EmailSettings:SenderName"]!,
                _config["EmailSettings:SenderEmail"]!));

            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };

            // FIX: Changed to the proper method call
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            // Added '!' to silence the null warnings
            await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"]!, int.Parse(_config["EmailSettings:SmtpPort"]!), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["EmailSettings:SenderEmail"]!, _config["EmailSettings:SenderPassword"]!);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}