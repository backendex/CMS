namespace CMS.src.Application.Services
{
    using CMS.src.Application.Interfaces;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;

    public class ResendEmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _from;

        public ResendEmailService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;

            _apiKey = config["Resend:ApiKey"]
                ?? throw new Exception("Resend:ApiKey no configurada");

            _from = config["Resend:FromEmail"]
                ?? throw new Exception("Resend:FromEmail no configurado");
        }
        public async Task SendEmailAsync(string to, string subject, string html)
        {
            var payload = new
            {
                from = _from,
                to = new[] { to },
                subject,
                html
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.resend.com/emails"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Resend error: {body}");

        }

        public async Task SendWelcomeEmail(
            string email,
            string fullName,
            string tempPassword,
            string confirmationLink
    )
        {
            var html = $@"
        <!DOCTYPE html>
        <html lang='es'>
        <head>
          <meta charset='UTF-8' />
          <meta name='viewport' content='width=device-width, initial-scale=1.0' />
          <title>Activa tu cuenta</title>
        </head>
        <body style='margin:0;padding:0;background-color:#f4f6f8;font-family:Arial,Helvetica,sans-serif;'>

          <table width='100%' cellpadding='0' cellspacing='0'>
            <tr>
              <td align='center' style='padding:40px 0;'>

                <!-- Card -->
                <table width='100%' max-width='600' style='background:#ffffff;border-radius:8px;padding:32px;box-shadow:0 4px 12px rgba(0,0,0,0.08);'>
          
                  <tr>
                    <td align='center'>
                      <h2 style='color:#111827;margin-bottom:8px;'>¡Bienvenido, {fullName}! 👋</h2>
                      <p style='color:#6b7280;font-size:15px;margin-top:0;'>
                        Tu cuenta ha sido creada exitosamente.
                      </p>
                    </td>
                  </tr>

                  <tr>
                    <td style='padding:24px 0;'>
                      <p style='color:#374151;font-size:15px;'>
                        Hemos generado una <strong>contraseña temporal</strong> para que puedas acceder:
                      </p>

                      <div style='background:#f3f4f6;border-radius:6px;padding:12px;text-align:center;font-size:18px;letter-spacing:1px;font-weight:bold;color:#111827;'>
                        {tempPassword}
                      </div>
                    </td>
                  </tr>

                  <tr>
                    <td style='padding-bottom:24px;'>
                      <p style='color:#374151;font-size:15px;'>
                        Para activar tu cuenta, haz clic en el siguiente botón:
                      </p>

                      <div style='text-align:center;margin:32px 0;'>
                        <a href='{confirmationLink}'
                           style='background:#2563eb;color:#ffffff;text-decoration:none;padding:14px 28px;border-radius:6px;font-weight:bold;font-size:15px;display:inline-block;'>
                          Activar cuenta
                        </a>
                      </div>

                      <p style='color:#6b7280;font-size:13px;'>
                        Este enlace es de un solo uso y puede expirar.
                      </p>
                    </td>
                  </tr>

                  <tr>
                    <td style='border-top:1px solid #e5e7eb;padding-top:16px;'>
                      <p style='color:#9ca3af;font-size:12px;text-align:center;'>
                        Si no solicitaste esta cuenta, puedes ignorar este correo.<br />
                        © {DateTime.Now.Year} Tu Plataforma
                      </p>
                    </td>
                  </tr>

                </table>

              </td>
            </tr>
          </table>

        </body>
        </html>
        ";

            await SendEmailAsync(
                email,
                "Activa tu cuenta",
                html
            );
        }

    }

}
