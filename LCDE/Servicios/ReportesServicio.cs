using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.NETCore;
using System.Reflection;

namespace LCDE.Servicios
{
    public class ReportesServicio
    {
        private readonly IConfiguration configuration;
        private readonly IFileRepository fileRepository;
        private readonly string invoiceDir;

        public ReportesServicio(
            IConfiguration configuration,
            IFileRepository fileRepository
            )
        {
            this.configuration = configuration;
            this.fileRepository = fileRepository;
            this.invoiceDir = configuration.GetValue<string>("Storage:InvoiceDir");
        }

        public async Task<string> CrearFactura(int idFactura)
        {
            Dictionary<string, string> parameters = new();
            Dictionary<string, object> dataSources = new();
            try
            {
                using SqlConnection sqlConnection = new(configuration.GetConnectionString("ConnectionLCDE"));
                // Obtener datos del encabezado
                var encabezadoFactura = await sqlConnection
                                            .QueryAsync
                                             (@"EXEC SP_CRUD_FACTURA @IdEncabezado, @Serie,
                                      @Fecha, @IdTipoPago, @IdCliente, @EstadoFacturaId, @Operacion",
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

                encabezadoFactura.First().QrImagen = "";
                // Llenado de dataSources encabezado.
                dataSources.Add("encabezado", encabezadoFactura);
                // Llenado de parámetros. encabezado.
                parameters.Add("IdEncabezado", idFactura.ToString());
                parameters.Add("Serie", "serie");
                parameters.Add("Fecha", DateTime.Now.ToString());
                parameters.Add("IdTipoPago", "0");
                parameters.Add("IdCliente", "0");
                parameters.Add("Operacion", "select");

                // Obtener datos de los detalles.
                var detallesFactura = await sqlConnection.QueryAsync(@"
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
                // Llenado de dataSources detalles factura.
                dataSources.Add("detalles", detallesFactura);
                // Llenado de parámetros. detalles factura.
                parameters.Add("IdDetalleFactura", "0");
                parameters.Add("Subtotal", "0");
                parameters.Add("Cantidad", "0");
                parameters.Add("IdProducto", "0");
                parameters.Add("IdEncabezadoFactura", idFactura.ToString());
                parameters.Add("DescuentoTotal", "0");
                parameters.Add("Operacion_detalle", "todo");

                // Ruta del archivo .rdl
                string fileDirPath = Assembly.GetExecutingAssembly().Location.Replace("LCDE.dll", string.Empty);
                string rdlcFilePath = string.Format("{0}Reportes\\{1}.rdl", fileDirPath, "FacturaVenta");
                using FileStream fileStream = new(rdlcFilePath, FileMode.Open, FileAccess.Read);

                using StreamReader reportDefinition = new(fileStream);
                LocalReport report = new();
                report.LoadReportDefinition(reportDefinition);

                foreach (var data in dataSources)
                {
                    report.DataSources.Add(new ReportDataSource(data.Key, data.Value));
                }
                foreach (var parametro in parameters)
                {
                    report.SetParameters(new[] { new ReportParameter(parametro.Key, parametro.Value) });
                }
                byte[] invoiceBytes = report.Render("PDF");

                // Convertir de byte[] a IFormFile
                using var ms = new MemoryStream(invoiceBytes);
                ms.Position = 0;
                IFormFile file = new FormFile(ms, 0, invoiceBytes.Length, $"Factura_{idFactura}.pdf", $"Factura_{idFactura}.pdf")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/pdf"
                };

                // Guardar archivo en Azure
                return await fileRepository.AddFile(file, invoiceDir);
            }
            catch
            {
                throw;
            }
        }
    }
}