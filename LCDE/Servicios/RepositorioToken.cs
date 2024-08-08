using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Security.Cryptography;

namespace LCDE.Servicios
{
    public interface IRepositorioToken
    {
        string CrearTokenRegistroUsuario(Usuario usuario);
        UsuarioToken? ObtenerToken(string token);
    }
    public class RepositorioToken : IRepositorioToken
    {
        private readonly IEncryptService encryptService;

        private readonly string connectionString;

        public RepositorioToken(IEncryptService encryptService, IConfiguration configuration)
        {
            this.encryptService = encryptService;
            connectionString = configuration.GetConnectionString("ConnectionLCDE");
        }
        public string CrearTokenRegistroUsuario(Usuario usuario)
        {
            var randomNumber = new byte[256];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var tokenStr = Convert.ToBase64String(randomNumber);
            var hashToken = encryptService.HashString($"{tokenStr}{usuario.Id}");

            using var connection = new SqlConnection(connectionString);
            connection.Execute(@"
                        EXEC crear_token_usuario @tipo, @token, @id_usuario, @fecha_solicitud, @fecha_vencimiento
                        ", new
            {
                tipo = "Confirmacion",
                token = hashToken,
                id_usuario = usuario.Id,
                fecha_solicitud = DateTime.Now,
                fecha_vencimiento = DateTime.Now.AddMinutes(15),
            });

            return hashToken;
        }


        public UsuarioToken? ObtenerToken(string token)
        {
            using var connection = new SqlConnection(connectionString);
            var tokenbd = connection.QueryFirstOrDefault<UsuarioToken>(@"
                        EXEC Obtener_token_por_valor , @token
                        ", new
            {
                token = token,
            });

            return tokenbd;
        }
    }
}
