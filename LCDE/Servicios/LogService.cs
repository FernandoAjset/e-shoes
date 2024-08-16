using Dapper;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public interface ILogService
    {
        void LogError(Exception ex);
    }

    public class LogService : ILogService
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        public LogService(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration.GetConnectionString("ConnectionLCDE");
        }

        public void LogError(Exception ex)
        {
            // guardar error en base de datos usando dapper.
            using var conexion = new SqlConnection(connectionString);
            conexion.Execute("INSERT INTO LogError (Message, StackTrace, Date) VALUES (@Message, @StackTrace, @Date)", new
            {
                ex.Message,
                ex.StackTrace,
                Date = DateTime.Now
            });
        }
    }
}