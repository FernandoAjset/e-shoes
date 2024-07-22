using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public class RepositorioDevoluciones
    {
        private readonly string connectionString;
        public RepositorioDevoluciones(IConfiguration configuration) //cosntructor que se llama igual que la clase
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");
        }

        public async Task<int> CrearDevolucion(Devolucion devolucion)
        {
            using var connection = new SqlConnection(connectionString);
            int devolucion_id = await connection.QuerySingleAsync<int>(@"
                EXEC SP_CRUD_DEVOLUCIONES @IdDevolucion, @Cantidad, @Motivo, @Id_producto, @Fecha, @Tipo_devolucion, @Operacion
                ", new
            {
                IdDevolucion = 0,
                Cantidad = devolucion.Cantidad,
                Motivo = devolucion.Motivo,
                Id_producto = devolucion.Id_producto,
                Fecha = devolucion.Fecha,
                Tipo_devolucion = devolucion.Tipo_devolucion,
                Operacion = "insert"
            });
            return devolucion_id;
        }

        public async Task<Devolucion> ObtenerDevolucion(int IdDevolucion)
        {
            using var connection = new SqlConnection(connectionString);
            Devolucion devolucion = await connection.QuerySingleAsync<Devolucion>(@"
                EXEC SP_CRUD_DEVOLUCIONES @IdDevolucion, @Cantidad, @Motivo, @Id_producto, @Fecha, @Tipo_devolucion, @Operacion
                ", new
            {
                IdDevolucion = IdDevolucion,
                Cantidad = "",
                Motivo = "",
                Id_producto = "",
                Fecha = "",
                Tipo_devolucion = "",
                Operacion = "select"

            });
            return devolucion;
        }

        public async Task<bool> ModificarDevolucion(Devolucion devolucion)
        {
            using var connection = new SqlConnection(connectionString);
            int devolucion_id = await connection.QuerySingleAsync<int>(@"
                EXEC SP_CRUD_DEVOLUCIONES @IdDevolucion, @Cantidad, @Motivo, @Id_producto, @Fecha, @Tipo_devolucion, @Operacion
                 ", new
            {
                IdDevolucion = devolucion.Id,
                Cantidad = devolucion.Cantidad,
                Motivo = devolucion.Motivo,
                Id_producto = devolucion.Id_producto,
                Fecha = devolucion.Fecha,
                Tipo_devolucion = devolucion.Tipo_devolucion,
                Operacion = "update"

            });
            return devolucion_id > 0;
        }

        public async Task<bool> BorrarDevolucion(int IdDevolucion)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                await connection.ExecuteAsync(@"
                EXEC SP_CRUD_DEVOLUCIONES @IdDevolucion, @Cantidad, @Motivo, @Id_producto, @Fecha, @Tipo_devolucion, @Operacion
                 ", new
                {
                    IdDevolucion = IdDevolucion,
                    Cantidad = "",
                    Motivo = "",
                    Id_producto = "",
                    Fecha = "",
                    Tipo_devolucion = "",
                    Operacion = "delete"

                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<DevolucionCrear>> ObtenerTodosDevoluciones()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<DevolucionCrear> devolucion = await connection.QueryAsync<DevolucionCrear>(@"
                EXEC SP_CRUD_DEVOLUCIONES @IdDevolucion, @Cantidad, @Motivo, @Id_producto, @Fecha, @Tipo_devolucion, @Operacion
                 ", new
            {
                IdDevolucion = 0,
                Cantidad = "",
                Motivo = "",
                Id_producto = "",
                Fecha = "",
                Tipo_devolucion = "",
                Operacion = "todo"

            });
            return devolucion;
        }

    }
}
