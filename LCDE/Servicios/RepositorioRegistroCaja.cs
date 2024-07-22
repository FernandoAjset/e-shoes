using Azure;
using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public class RepositorioRegistroCaja
    {
        private readonly string connectionString;
        public RepositorioRegistroCaja(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");

        }

        public async Task<int> CrearRegistroCaja(RegistroCaja registroCajas)
        {
            using var connection = new SqlConnection(connectionString);
            int registro_id = await connection.QuerySingleAsync<int>(@"
            EXEC SP_REGISTRO_CAJA @IdCaja, @Fecha, @Turno, 
            @IdUsuario, @TipoRegistro, @Monto, @Operacion
            ", new
            {
                IdCaja = registroCajas.IdCaja,
                Fecha = DateTime.Now,
                Turno = registroCajas.Turno,
                IdUsuario = registroCajas.IdUsuario,
                TipoRegistro = registroCajas.TipoRegistro,
                Monto = registroCajas.Monto,
                Operacion = "insert"
            });
            return registro_id;
        }


        public async Task<IEnumerable<RegistroCaja>> ObtenerTodosRegistros()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<RegistroCaja> registroCajas = await connection.QueryAsync<RegistroCaja>(@"
            EXEC SP_REGISTRO_CAJA @IdRegistro, @Fecha, @Turno, @IdCaja, @IdUsuario, @TipoRegistro, @Monto, @Operacion 
            ", new
            {
                IdRegistro = "",
                Fecha = "",
                Turno = "",
                IdCaja = "",
                IdUsuario = "",
                TipoRegistro = "",
                Monto = "",
                Operacion = "todo"

            });
            return registroCajas;
        }
    }
}
