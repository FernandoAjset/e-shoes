using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LCDE.Servicios
{
    public class RepositorioVentas
    {
        private readonly string connectionString;
        public RepositorioVentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");

        }
        public async Task<int> CrearVenta(VentaViewModel venta)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                int factura_id = 0;
                factura_id = await connection.QuerySingleAsync<int>(@"
                        EXEC SP_CRUD_FACTURA 
                        @IdEncabezado, @Serie, @Fecha,
                        @IdTipoPago, @IdCliente, @Operacion
                        ", new
                {
                    IdEncabezado = 0,
                    Serie = venta.EncabezadoFactura.Serie,
                    Fecha = DateTime.Now,
                    IdTipoPago = venta.EncabezadoFactura.IdTipoPago,
                    IdCliente = venta.EncabezadoFactura.IdCliente,
                    Operacion = "insert"
                }, transaction);
                foreach (var detalle in venta.DetallesFactura)
                {
                    await connection.ExecuteAsync(@"
                        EXEC SP_DETALLE_FACTURA 
                        @IdDetalleFactura, @Subtotal, @Cantidad, 
                        @IdProducto, @IdEncabezadoFactura, @DescuentoTotal,
                        @Operacion
                        ", new
                    {
                        IdDetalleFactura = 0,
                        Subtotal = detalle.Subtotal,
                        Cantidad = detalle.Cantidad,
                        IdProducto = detalle.IdProducto,
                        IdEncabezadoFactura = factura_id,
                        DescuentoTotal = detalle.Descuento,
                        Operacion = "insert"
                    }, transaction);
                }
                await transaction.CommitAsync();
                return factura_id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return 0;
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<Cliente> ObtenerCliente(int IdCliente)
        {
            using var connection = new SqlConnection(connectionString);
            Cliente cliente = await connection.QuerySingleAsync<Cliente>(@"
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @NIT,@IdCliente, @Operacion
                        ", new
            {
                NombreCliente = "",
                DireccionCliente = "",
                TelefonoCliente = "",
                CorreoCliente = "",
                NIT = "",
                IdCliente = IdCliente,
                Operacion = "select"
            });
            return cliente;
        }
        public async Task<Cliente> ObtenerClientePorNit(string NIT, int Id)
        {
            Cliente cliente = new();
            try
            {
                using var connection = new SqlConnection(connectionString);
                cliente = await connection.QueryFirstOrDefaultAsync<Cliente>(@"
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @NIT, @IdCliente,@Operacion
                        ", new
                {
                    NombreCliente = "",
                    DireccionCliente = "",
                    TelefonoCliente = 0,
                    CorreoCliente = "",
                    NIT = NIT,
                    IdCliente = Id,
                    Operacion = "selectPorNit"
                });
                return cliente;
            }
            catch (Exception ex)
            {
                return cliente;
            }
        }
        public async Task<bool> ModificarCliente(Cliente cliente)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync(@"
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @NIT,@IdCliente, @Operacion
                        ", new
                {
                    NombreCliente = cliente.Nombre,
                    DireccionCliente = cliente.Direccion,
                    TelefonoCliente = cliente.Telefono,
                    CorreoCliente = cliente.Correo,
                    NIT = cliente.NIT,
                    IdCliente = cliente.Id,
                    Operacion = "update"
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> BorrarCliente(int IdCliente)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync(@"
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @NIT, @IdCliente, @Operacion
                        ", new
                {
                    NombreCliente = "",
                    DireccionCliente = "",
                    TelefonoCliente = "",
                    CorreoCliente = "",
                    NIT = "",
                    IdCliente,
                    Operacion = "delete"
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<IEnumerable<Cliente>> ObtenerTodosClientes()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<Cliente> clientes = await connection.QueryAsync<Cliente>(@"
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @NIT, @IdCliente, @Operacion
                        ", new
            {
                NombreCliente = "",
                DireccionCliente = "",
                TelefonoCliente = 0,
                CorreoCliente = "",
                NIT = "",
                IdCliente = 0,
                Operacion = "todo"
            });
            return clientes;
        }
    }
}
