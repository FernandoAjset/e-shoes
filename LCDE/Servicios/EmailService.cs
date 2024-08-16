using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LCDE.Servicios
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogService logService;
        private readonly IConfiguration configuration;

        public EmailService(
            ILogService logService,
            IConfiguration configuration)
        {
            this.logService = logService;
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var newEmail = new MimeMessage();
                newEmail.From.Add(MailboxAddress.Parse("eshoes-clangpt"));
                newEmail.To.Add(MailboxAddress.Parse(email));
                newEmail.Subject = subject;

                // Crear el cuerpo del correo con formato HTML
                newEmail.Body = new TextPart("html")
                {
                    Text = message
                };

                using var smtp = new SmtpClient();

                // Ignorar la validación del certificado en entornos de desarrollo
                smtp.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                smtp.Connect(
                    configuration["SMTP:HOST"],
                    Convert.ToInt32(configuration["SMTP:PORT"]),
                    SecureSocketOptions.StartTls
                );

                smtp.Authenticate(
                    configuration["SMTP:USERNAME"],
                    configuration["SMTP:PASSWORD"].Replace('*', ' ') // Reemplazar asteriscos por espacios
                );

                await smtp.SendAsync(newEmail);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        private void LogError(Exception ex)
        {
            try
            {
                logService.LogError(ex);
            }
            catch
            {
                throw;
            }
        }
    }
}