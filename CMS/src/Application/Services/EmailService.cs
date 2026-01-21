using CMS.src.Application.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace CMS.src.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config) => _config = config;

        public async Task SendWelcomeEmail(string email, string password, string confirmationLink)
        {
            string subject = "Bienvenido al CMS - Activa tu cuenta";

            string body = $@"
            <h1>¡Bienvenido!</h1>
            <p>Se ha creado una cuenta para ti en el sistema.</p>
            <p><strong>Tu contraseña temporal es:</strong> {password}</p>
            <br>
            <p>Para activar tu cuenta y poder iniciar sesión, haz clic en el siguiente botón:</p>
            <a href='{confirmationLink}' style='background-color: #3498db; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                Activar mi cuenta
            </a>
            <p>Si el botón no funciona, copia y pega este enlace: {confirmationLink}</p>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();

            // Esto está bien, ignora certificados no válidos
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await smtp.ConnectAsync(
                _config["EmailSettings:Server"],
                int.Parse(_config["EmailSettings:Port"]!),
                MailKit.Security.SecureSocketOptions.None // <-- CAMBIA 'Auto' por 'None'
            );

            if (!string.IsNullOrEmpty(_config["EmailSettings:Username"]))
            {
                await smtp.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
            }

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

    }
}
