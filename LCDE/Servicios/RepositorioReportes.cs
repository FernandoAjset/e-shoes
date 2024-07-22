using Dapper;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public class RepositorioReportes
    {
        private readonly string connectionString;
        public RepositorioReportes(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");

        }

        public async Task<IEnumerable<dynamic>> VentasDiariasPorCategoria(string fecha)
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<dynamic> registroCajas = await connection.QueryAsync<dynamic>(@"
            EXEC SP_REPORTES
                @Fecha,
                @Turno,
                @Categoria,
                @Operacion
            ", new
            {
                Fecha = fecha,
                Turno = "",
                Categoria = "",
                Operacion = "VentasDiariasPorCategoria"

            });
            return registroCajas;
        }
        public async Task<IEnumerable<dynamic>> CorteCajaPorTurno()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<dynamic> registroCajas = await connection.QueryAsync<dynamic>(@"
            EXEC SP_REPORTES
                @Fecha,
                @Turno,
                @Categoria,
                @Operacion
            ", new
            {
                Fecha = DateTime.Now,
                Turno = "",
                Categoria = "",
                Operacion = "CorteCajaPorTurno"

            });
            return registroCajas;
        }
        public async Task<IEnumerable<dynamic>> VentasPorCategoria(string categoria, string fecha)
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<dynamic> registroCajas = await connection.QueryAsync<dynamic>(@"
            EXEC SP_REPORTES
                @Fecha,
                @Turno,
                @Categoria,
                @Operacion
            ", new
            {
                Fecha = fecha,
                Turno = "",
                Categoria = categoria,
                Operacion = "VentasPorCategoria"

            });
            return registroCajas;
        }
        public async Task<IEnumerable<dynamic>> DevolucionesPorCategoria(string categoria, string fecha)
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<dynamic> registroCajas = await connection.QueryAsync<dynamic>(@"
            EXEC SP_REPORTES
                @Fecha,
                @Turno,
                @Categoria,
                @Operacion
            ", new
            {
                Fecha = fecha,
                Turno = "",
                Categoria = categoria,
                Operacion = "DevolucionesPorCategoria"

            });
            return registroCajas;
        }
    }
}
