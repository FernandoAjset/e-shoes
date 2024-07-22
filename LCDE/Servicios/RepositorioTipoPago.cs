using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{ 
    public class RepositorioTipoPago
    {
    private readonly string connectionString;
        public RepositorioTipoPago(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");
        }

        public async Task<IEnumerable<TipoPago>> ObtenerTodosTipoPago()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<TipoPago> tipoPagos = await connection.QueryAsync<TipoPago>(@"
                        EXEC SP_TIPO_PAGO @IdTipoPago, @Tipo, @Operacion
                        ", new
            {
                IdTipoPago = 0,
                Tipo = "",
                Operacion = "todo"

            });
            return tipoPagos;
        }

    }
}

