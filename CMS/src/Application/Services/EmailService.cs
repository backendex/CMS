using CMS.src.Application.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace CMS.src.Application.Services
{
    public class EmailService : IEmailService
    {
        // Implementación del servicio de correo electrónico
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config) => _config = config;


        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeKit.MimeMessage();
            // Verifica que SenderName y SenderEmail coincidan con tu JSON
            email.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();

            // CAMBIO AQUÍ: "Server" en lugar de "SmtpServer"
            await smtp.ConnectAsync(
                _config["EmailSettings:Server"],
                int.Parse(_config["EmailSettings:Port"]!),
                MailKit.Security.SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
