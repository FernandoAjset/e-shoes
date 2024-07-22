using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using Microsoft.Reporting.NETCore;
using Microsoft.ReportingServices.Diagnostics.Internal;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Transactions;

namespace LCDE.Servicios
{
    public class ReportesServicio
    {
        private readonly IConfiguration configuration;

        public ReportesServicio(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<byte[]> CrearFactura(
            int idFactura,
            string url
            )
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

                // Generar QR y convertirlo a imagen
                QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
                QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(url, QRCoder.QRCodeGenerator.ECCLevel.Q);
                QRCoder.BitmapByteQRCode qRCode = new QRCoder.BitmapByteQRCode(qRCodeData);
                var bmpBytes = qRCode.GetGraphic(7);
                // Convierte el arreglo de bytes a un objeto Bitmap
                using MemoryStream stream = new MemoryStream(bmpBytes);
                using MemoryStream streamBitMaP = new MemoryStream();

                using Bitmap bitmap = new Bitmap(stream);
                bitmap.Save(streamBitMaP, ImageFormat.Bmp);

                var base64 = Convert.ToBase64String(streamBitMaP.ToArray());
                encabezadoFactura.First().QrImagen = base64;
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
                return report.Render("PDF");

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
