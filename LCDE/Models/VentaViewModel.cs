using Microsoft.AspNetCore.Mvc.Rendering;

namespace LCDE.Models
{
    public class VentaViewModel
    {
        public EncabezadoFactura EncabezadoFactura { get; set; }
        public List<DetalleFactura>? DetallesFactura { get; set; }
    }
}
