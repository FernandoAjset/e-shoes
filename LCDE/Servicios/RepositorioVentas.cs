using Dapper;
using LCDE.Models;
using LCDE.Models.Enums;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public class RepositorioVentas
    {
        private readonly string connectionString;
        private readonly IEmailService emailService;

        public RepositorioVentas(IConfiguration configuration, IEmailService emailService)
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");
            this.emailService = emailService;
        }

        public async Task<EncabezadoFactura> ObtenerEncabezadoFacturaPorId(int idFactura)
        {
            using var connection = new SqlConnection(connectionString);
            // Obtener datos del encabezado
            return await connection
                                    .QueryFirstOrDefaultAsync<EncabezadoFactura>
                                     (@"EXEC SP_CRUD_FACTURA @IdEncabezado, @Serie,
                                          @Fecha, @IdTipoPago, @IdCliente, 
                                          @EstadoFacturaId, @Operacion",
                new
                {
                    IdEncabezado = idFactura,
                    Serie = "",
                    Fecha = DateTime.Now,
                    IdTipoPago = 0,
                    IdCliente = 0,
                    EstadoFacturaId = 0,
                    Operacion = "select"
                });
        }

        public async Task<EncabezadoFactura> ObtenerFacturaPorTokenPago(string token)
        {
            using var connection = new SqlConnection(connectionString);
            // Obtener datos del encabezado
            return await connection
                                    .QueryFirstOrDefaultAsync<EncabezadoFactura>
                                    (@$"SELECT encabezado_factura.*, clientes.correo CorreoCliente
                                        FROM encabezado_factura
                                        INNER JOIN clientes
                                        ON encabezado_factura.id_cliente=clientes.id
                                        WHERE encabezado_factura.token_pago=@token",
                                        new { token });
        }

        public async Task<IEnumerable<EncabezadoFactura>> ObtenerFacturasPorCliente(int idCliente)
        {
            using var connection = new SqlConnection(connectionString);
            // Obtener datos del encabezado
            return await connection
                                    .QueryAsync<EncabezadoFactura>
                                    (@"
                                        SELECT 
                                            encabezado_factura.*, 
                                            factura_estado.nombre AS estado,
	                                        encabezado_factura.estado_factura_id AS estadoFacturaId,
	                                        tipo_pago.tipo AS tipoPago,
                                            (SELECT SUM(detalle_factura.subtotal) 
                                             FROM detalle_factura 
                                             WHERE detalle_factura.id_encabezado_factura = encabezado_factura.id) AS total
                                        FROM 
                                            encabezado_factura
                                        INNER JOIN 
                                            factura_estado ON encabezado_factura.estado_factura_id = factura_estado.id
                                        INNER JOIN
	                                        tipo_pago ON tipo_pago.id=encabezado_factura.id_tipo_pago
                                        WHERE 
                                            encabezado_factura.id_cliente = @idCliente
                                        AND encabezado_factura.estado_factura_id!= 3 -- 3. Anulada
                                        ORDER BY fecha DESC;
                                    ",
                new
                {
                    idCliente
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
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                int factura_id = await connection.QuerySingleAsync<int>(@"
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
                await this.CheckStock(venta.DetallesFactura);
                return factura_id;
            }
            catch
            {
                throw;
            }
        }

        public async Task AgregarUrlFactura(string url, int idFactura)
        {
            try
            {
                var connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync(@"
                        UPDATE encabezado_factura
                        SET url = @url
                        WHERE id = @idFactura", new { url, idFactura });
            }
            catch
            {
                throw;
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
        public async Task<bool> AgregarInfoPagoFactura(int idFactura, PaypalOrderResponse data)
        {
            using var connection = new SqlConnection(connectionString);
            var result = await connection.ExecuteAsync(@"
                UPDATE encabezado_factura
                SET token_pago = @tokenPago,
                url_pago = @urlPago
                WHERE id = @idFactura", new
            {
                idFactura,
                tokenPago = data.id,
                urlPago = data.links.FirstOrDefault(l => l.rel == "approve")?.href ?? ""
            });

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

        public async Task AnularFactura(int idFactura)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"EXEC SP_ANULAR_FACTURA @IdEncabezado", new { IdEncabezado = idFactura });

        }

        private async Task CheckStock(List<DetalleFactura> detalles)
        {
            try
            {

                using var connection = new SqlConnection(connectionString);

                var admins = (await connection.QueryAsync<Usuario>(@"
                    select * from usuarios where Id_role = 1;")).ToList();

                foreach (var item in detalles)
                {

                    var producto = (await connection.QuerySingleAsync<Producto>(@"
                    select * from productos where id = @IdProducto;", new { item.IdProducto }));

                    if (producto.Existencia <= producto.Stock_Minimo)
                    {


                        string asunto = $"Alerta de stock bajo para el producto {producto.Nombre}";
                        string mensaje = $@"
                    <h1>Alerta de stock</h1>
                    <p>El stock del producto <strong>{producto.Nombre}</strong> es menor al recomendado.</p>
                    <p>Stock actual: {producto.Existencia}</p>
                    <p>Stock mínimo recomendado: {producto.Stock_Minimo}</p>";

                        foreach (var admin in admins)
                        {
                            await emailService.SendEmailAsync(admin.Correo, asunto, mensaje);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}