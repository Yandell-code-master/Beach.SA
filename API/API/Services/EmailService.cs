using System.Net;
using System.Net.Mail;

namespace API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task EnviarCorreoConAdjuntoAsync(
            string destinatario,
            string asunto,
            string cuerpo,
            byte[] adjunto,
            string nombreAdjunto)
        {
            string host = _configuration["EmailSettings:Host"] ?? "";
            int port = _configuration.GetValue<int>("EmailSettings:Port", 587);
            string username = _configuration["EmailSettings:Username"] ?? "";
            string password = _configuration["EmailSettings:Password"] ?? "";
            string fromName = _configuration["EmailSettings:FromName"] ?? "Beach.SA - Hotel & Resort";
            string fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@beachsa.com";
            bool enableSsl = _configuration.GetValue<bool>("EmailSettings:EnableSsl", true);

            if (string.IsNullOrEmpty(host))
            {
                _logger.LogWarning("EmailService: servidor SMTP no configurado (EmailSettings:Host). No se envió el correo a {Destinatario}.", destinatario);
                return;
            }

            try
            {
                using var smtp = new SmtpClient(host, port);
                smtp.EnableSsl = enableSsl;
                smtp.Credentials = new NetworkCredential(username, password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Timeout = 30000;

                using var mensaje = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = asunto,
                    Body = cuerpo,
                    IsBodyHtml = true,
                    Priority = MailPriority.Normal
                };

                mensaje.To.Add(destinatario);

                var ms = new MemoryStream(adjunto);
                var attachment = new Attachment(ms, nombreAdjunto, "application/pdf");
                mensaje.Attachments.Add(attachment);

                await smtp.SendMailAsync(mensaje);

                _logger.LogInformation(
                    "EmailService: Correo enviado exitosamente a {Destinatario} con adjunto {Archivo}.",
                    destinatario, nombreAdjunto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "EmailService: Error al enviar correo a {Destinatario} con adjunto {Archivo}.",
                    destinatario, nombreAdjunto);
            }
        }
    }
}
