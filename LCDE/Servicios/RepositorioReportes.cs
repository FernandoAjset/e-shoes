using Dapper;
using LCDE.Models;
using LCDE.Models.Enums;
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

        public async Task<IEnumerable<FacturaDTOViewModel>> Facturas()
        {
            using var connection = new SqlConnection(connectionString);
            var registroCajas = await connection.QueryAsync<VistaFacturaDTO>(@"
                EXEC SP_REPORTES
                    @Fecha,
                    @Turno,
                    @Categoria,
                    @Operacion
            ", new
            {
                Fecha = "",
                Turno = "",
                Categoria = "",
                Operacion = "Facturas"
            });

            if (registroCajas == null)
            {
                return Enumerable.Empty<FacturaDTOViewModel>();
            }

            // Agrupamos los resultados por factura (id_factura)
            var facturasAgrupadas = registroCajas
                .GroupBy(f => f.Id_Factura) // O puedes usar otro valor por defecto
                .Select(g => new FacturaDTOViewModel
                {
                    IdFactura = g.Key, // id_factura como clave de agrupamiento
                    Serie = g.First().Serie,
                    Fecha = g.First().Fecha,
                    Estado = g.First().estado_factura_id,
                    Cliente = new ClienteDTOViewModel
                    {
                        Nombre = g.First().Nombre_cliente,
                        Direccion = g.First().Direccion,
                        Telefono = g.First().Telefono,
                        Correo = g.First().Correo,
                        NIT = g.First().NIT
                    },
                    Subtotal = g.First().Subtotal,
                    Url= g.First().Url,
                    Detalles = g.Select(d => new DetalleFacturaDTOViewModel
                    {
                        Cantidad = d.Cantidad,
                        NombreProducto = d.nombre_producto,
                        DescripcionProducto = d.descripcion_producto,
                        PrecioUnidad = d.precio_unidad,
                        Subtotal = d.Subtotal,
                        Categoria = d.Categoria,
                        ImageUrl = d.ImageUrl
                    }).ToList()
                }).ToList();

            return facturasAgrupadas;
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
