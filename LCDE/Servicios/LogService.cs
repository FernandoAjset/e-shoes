using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public interface ILogService
    {
        void Log(Log log);
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

        public void Log(Log log)
        {
            // guardar error en base de datos usando dapper.
            using var conexion = new SqlConnection(connectionString);
            conexion.Execute("INSERT INTO Log (Type, Message, StackTrace, Date) VALUES (@Type, @Message, @StackTrace, @Date)", new
            {
                log.Type,
                log.Message,
                log.StackTrace,
                log.Date
            });
        }
    }
}