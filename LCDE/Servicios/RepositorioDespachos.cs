    using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public class RepositorioDespachos
    {
        private readonly string connectionString;
        public RepositorioDespachos(IConfiguration configuration) //cosntructor que se llama igual que la clase
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");
        }

        public async Task<int> CrearDespacho(Despacho despacho)
        {
            using var connection = new SqlConnection(connectionString);
            int despacho_id = await connection.QuerySingleAsync<int>(@"
                EXEC SP_CRUD_DESPACHOS @IdDespacho, @IdEncabezadoFactura, @FechaDespacho, @operacion
                ", new
            {
                IdEncabezadoFactuta = despacho.IdEncabezadoFactura,
                FechaDespacho = despacho.FechaDespacho,
                IdDespacho = 0,
                Operacion = "insert"
            });
            return despacho_id;
        }

        public async Task<bool> BorrarDespacho(int IdDespacho)
        {
            using var connection = new SqlConnection(connectionString);
            int despacho = await connection.QuerySingleAsync<int>(@"
                EXEC SP_CRUD_DESAPACHOS @IdDespacho, @IdEncabezadoFactura, @FechaDespacho, @operacion
                ", new
            {
                NIdEncabezadoFactuta = "",
                FechaDespacho = "",
                IdDespacho = IdDespacho,
                Operacion = "delete"

            });
            return despacho > 0;
        }

        public async Task<IEnumerable<Despacho>> ObtenerTodoDespacho()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<Despacho> despacho = await connection.QueryAsync<Despacho>(@"
                EXEC SP_CRUD_DESAPACHOS @IdDespacho, @IdEncabezadoFactura, @FechaDespacho, @operacion
                ", new
            {
                NIdEncabezadoFactuta = "",
                FechaDespacho = "",
                IdDespacho = 0,
                Operacion = "todo"

            });
            return despacho;
        }
    }
}
