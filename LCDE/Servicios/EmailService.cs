using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace LCDE.Servicios
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var newEmail = new MimeMessage();
                newEmail.From.Add(MailboxAddress.Parse(configuration.GetSection("SMTP:USERNAME").Value));
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
                    configuration.GetSection("SMTP:HOST").Value,
                    Convert.ToInt32(configuration.GetSection("SMTP:PORT").Value),
                    SecureSocketOptions.StartTls
                );

                smtp.Authenticate(
                    configuration.GetSection("SMTP:USERNAME").Value,
                    configuration.GetSection("SMTP:PASSWORD").Value
                );

                smtp.Send(newEmail);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}