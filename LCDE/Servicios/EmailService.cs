using RestSharp;
using RestSharp.Authenticators;
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

                var client = new RestClient(new RestClientOptions
                {
                    BaseUrl = new Uri("https://api.mailgun.net/v3"),
                    Authenticator = new HttpBasicAuthenticator("api", apiKey)
                });

                var request = new RestRequest("sandbox64a0a36a7c454c5086d74f985001af3d.mailgun.org/messages", Method.Post);
                request.AddParameter("domain", "fernandoajset.studio", ParameterType.UrlSegment);
                request.AddParameter("from", emailRemitente);
                request.AddParameter("to", email);
                request.AddParameter("subject", subject);
                request.AddParameter("text", message);

                var response = await client.ExecuteAsync(request);
                Console.WriteLine(response.Content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                throw;
            }
        }
    }
}