using SendGrid;
using SendGrid.Helpers.Mail;

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
                var apiKey = configuration.GetValue<string>("SENDGRID_API_KEY");
                var emailRemitente = configuration.GetValue<string>("SENDGRID_FROM");
                var nombreRemitente = configuration.GetValue<string>("SENDGRID_NOMBRE");

                var cliente = new SendGridClient(apiKey);
                var from = new EmailAddress(emailRemitente, nombreRemitente);
                var subjectMail = subject;
                var to = new EmailAddress(email, email);
                var contenidoHtml = message;
                var singleEmail = MailHelper.CreateSingleEmail(from, to, subject,
                                    "", contenidoHtml);
                var respuesta = await cliente.SendEmailAsync(singleEmail);
            }
            catch
            {
                throw;
            }
        }
    }
}