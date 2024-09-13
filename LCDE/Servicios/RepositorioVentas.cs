using Dapper;
using LCDE.Models;
using LCDE.Models.Enums;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public class RepositorioVentas
    {
        private readonly string connectionString;
        public RepositorioVentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");

        }

        public async Task<EncabezadoFactura> ObtenerEncabezadoFacturaPorId(int idFactura)
        {
            using var connection = new SqlConnection(connectionString);
            // Obtener datos del encabezado
            return await connection
                                    .QueryFirstOrDefaultAsync<EncabezadoFactura>
                                     (@"EXEC SP_CRUD_FACTURA @IdEncabezado, @Serie,
                                          @Fecha, @IdTipoPago, @IdCliente, @Operacion",
                new
                {
                    IdEncabezado = idFactura,
                    Serie = "",
                    Fecha = DateTime.Now,
                    IdTipoPago = 0,
                    IdCliente = 0,
                    Operacion = "select"
                });
        }

        public async Task<IEnumerable<DetalleFactura>> ObtenerDetallesFactura(int idFactura)
        {
            using var connection = new SqlConnection(connectionString);

            // Obtener datos de los detalles.
            return await connection.QueryAsync<DetalleFactura>(@"
                        EXEC SP_DETALLE_FACTURA 
                        @IdDetalleFactura, @Subtotal, @Cantidad, 
                        @IdProducto, @IdEncabezadoFactura, @DescuentoTotal,
                        @Operacion
                        ", new
            {
                IdDetalleFactura = 0,
                Subtotal = 0,
                Cantidad = 0,
                IdProducto = 0,
                IdEncabezadoFactura = idFactura,
                DescuentoTotal = 0,
                Operacion = "todo"
            });
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
                        @IdTipoPago, @IdCliente, 
                        @EstadoFacturaId, @Operacion
                        ", new
                {
                    IdEncabezado = 0,
                    venta.EncabezadoFactura.Serie,
                    Fecha = DateTime.Now,
                    venta.EncabezadoFactura.IdTipoPago,
                    venta.EncabezadoFactura.IdCliente,
                    venta.EncabezadoFactura.EstadoFacturaId,
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
                        detalle.Subtotal,
                        detalle.Cantidad,
                        detalle.IdProducto,
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
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @Nit,@IdCliente, @Operacion
                        ", new
            {
                NombreCliente = "",
                DireccionCliente = "",
                TelefonoCliente = "",
                CorreoCliente = "",
                Nit = "",
                IdCliente,
                Operacion = "select"
            });
            return cliente;
        }
        public async Task<Cliente> ObtenerClientePorNit(string Nit, int Id)
        {
            Cliente cliente = new();
            try
            {
                using var connection = new SqlConnection(connectionString);
                cliente = await connection.QueryFirstOrDefaultAsync<Cliente>(@"
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @Nit, @IdCliente,@Operacion
                        ", new
                {
                    NombreCliente = "",
                    DireccionCliente = "",
                    TelefonoCliente = 0,
                    CorreoCliente = "",
                    Nit,
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
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @Nit,@IdCliente, @Operacion
                        ", new
                {
                    NombreCliente = cliente.Nombre,
                    DireccionCliente = cliente.Direccion,
                    TelefonoCliente = cliente.Telefono,
                    CorreoCliente = cliente.Correo,
                    Nit = cliente.Nit,
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
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @Nit, @IdCliente, @Operacion
                        ", new
                {
                    NombreCliente = "",
                    DireccionCliente = "",
                    TelefonoCliente = "",
                    CorreoCliente = "",
                    Nit = "",
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
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @Nit, @IdCliente, @Operacion
                        ", new
            {
                NombreCliente = "",
                DireccionCliente = "",
                TelefonoCliente = 0,
                CorreoCliente = "",
                Nit = "",
                IdCliente = 0,
                Operacion = "todo"
            });
            return clientes;
        }
        public async Task<bool> AgregarTokenPagoEnFactura(int idFactura, string tokenPago)
        {
            using var connection = new SqlConnection(connectionString);
            var result = await connection.ExecuteAsync(@"
                UPDATE encabezado_factura
                SET token_pago = @tokenPago
                WHERE id = @idFactura", new { idFactura, tokenPago });

            return result > 0;
        }

        public async Task<bool> ActualizarEstadoVentaPagado(string tokenPago)
        {
            using var connection = new SqlConnection(connectionString);
            var result = await connection.ExecuteAsync(@"
                UPDATE encabezado_factura
                SET estado_factura_id = @Estado
                WHERE token_pago = @tokenPago", new { Estado = (int)FacturaEstadoEnum.Pagada, tokenPago });

            return result > 0;
        }
    }
}
