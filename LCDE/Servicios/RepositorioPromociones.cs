using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public class RepositorioPromociones
    {
        private readonly string connectionString;
        public RepositorioPromociones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");

        }
        public async Task<IEnumerable<Promocion>> ObtenerTodosPromociones()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<Promocion> promocion = await connection.QueryAsync<Promocion>(@"
                        EXEC SP_TODAS_PROMOCIONES @IdPromociones, @Descripcion, @Operacion
                        ", new
            {
                IdPromociones = 0,
                Descripcion = "",
                Operacion = "todo"
            });
            return promocion;
        }

        public async Task<IEnumerable<dynamic>> PromocionesFactura(int IdFactura)
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<dynamic> promociones = await connection.QueryAsync<dynamic>(@"
                        EXEC SP_PROMOCIONES_FACTURA @IdFactura
                        ", new
            {
                IdFactura = IdFactura
            });
            return promociones;
        }

    }
}
