using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public class RepositorioProveedores // clase
    {
        private readonly string connectionString;
        public RepositorioProveedores(IConfiguration configuration) //cosntructor que se llama igual que la clase
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");
        }

        public async Task<int> CrearProveedor(Proveedor proveedor)
        {
            using var connection = new SqlConnection(connectionString);
            int proveedor_id = await connection.QuerySingleAsync<int>(@"
                EXEC SP_CRUD_PROVEEDORES @NombreProveedor, @DireccionProveedor, @TelefonoProveedor, 
                @CorreoProveedor, @NITProveedor, @IdProveedor, @Operacion
             ", new
            {
                NombreProveedor = proveedor.Nombre,
                DireccionProveedor = proveedor.Direccion,
                TelefonoProveedor = proveedor.Telefono,
                CorreoProveedor = proveedor.Correo,
                NITProveedor = proveedor.NIT,
                IdProveedor = 0,
                Operacion = "insert"
            });
            return proveedor_id;
        }

        public async Task<Proveedor> ObtenerProveedor(int IdProveedor)
        {
            using var connection = new SqlConnection(connectionString);
            Proveedor proveedor = await connection.QuerySingleAsync<Proveedor>(@"
                EXEC SP_CRUD_PROVEEDORES @NombreProveedor, @DireccionProveedor, @TelefonoProveedor, 
                @CorreoProveedor, @NITProveedor, @IdProveedor, @Operacion
                ", new
            {
                NombreProveedor = "",
                DireccionProveedor = "",
                TelefonoProveedor = "",
                CorreoProveedor = "",
                NITProveedor = "",
                IdProveedor = IdProveedor,
                Operacion = "select"

            });
            return proveedor;
        }
        public async Task<bool> ModificarProveedor(Proveedor proveedor)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                int proveedor_id = await connection.ExecuteAsync(@"
                 EXEC SP_CRUD_PROVEEDORES @NombreProveedor, @DireccionProveedor, @TelefonoProveedor, 
                 @CorreoProveedor, @NITProveedor, @IdProveedor, @Operacion
                 ", new
                {
                    NombreProveedor = proveedor.Nombre,
                    DireccionProveedor = proveedor.Direccion,
                    TelefonoProveedor = proveedor.Telefono,
                    CorreoProveedor = proveedor.Correo,
                    NITProveedor = proveedor.NIT,
                    IdProveedor = proveedor.Id,
                    Operacion = "update"

                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> BorrarProveedor(int IdProveedor)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync(@"
                        EXEC SP_CRUD_PROVEEDORES @NombreProveedor, @DireccionProveedor, @TelefonoProveedor, 
                        @CorreoProveedor, @NITProveedor, @IdProveedor, @Operacion
                        ", new
                {
                    NombreProveedor = "",
                    DireccionProveedor = "",
                    TelefonoProveedor = "",
                    CorreoProveedor = "",
                    NITProveedor = "",
                    IdProveedor = IdProveedor,
                    Operacion = "delete"

                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Proveedor>> ObtenerTodosProveedores()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<Proveedor> proveedor = await connection.QueryAsync<Proveedor>(@"
                        EXEC SP_CRUD_PROVEEDORES @NombreProveedor, @DireccionProveedor, @TelefonoProveedor, 
                        @CorreoProveedor, @NITProveedor, @IdProveedor, @Operacion
                        ", new
            {
                NombreProveedor = "",
                DireccionProveedor = "",
                TelefonoProveedor = "",
                CorreoProveedor = "",
                NITProveedor = "",
                IdProveedor = 0,
                Operacion = "todo"

            });
            return proveedor;
        }

        public async Task<Proveedor> ObtenerProveedorPorNit(string NIT, int Id)
        {
            Proveedor proveedor = new();
            try
            {
                using var connection = new SqlConnection(connectionString);
                proveedor = await connection.QueryFirstOrDefaultAsync<Proveedor>(@"
                        EXEC SP_CRUD_PROVEEDORES @NombreProveedor, @DireccionProveedor, @TelefonoProveedor, 
                        @CorreoProveedor, @NITProveedor, @IdProveedor, @Operacion
                        ", new
                {
                    NombreProveedor = "",
                    DireccionProveedor = "",
                    TelefonoProveedor = "",
                    CorreoProveedor = "",
                    NITProveedor = NIT,
                    IdProveedor = Id,
                    Operacion = "selectPorNit"
                });
                return proveedor;
            }
            catch (Exception ex)
            {
                return proveedor;
            }
        }

    }

}
