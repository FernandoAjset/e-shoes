namespace LCDE.Models
{
    public class ProductoListarDTO : Producto
    {
        public string NombreCategoria { get; set; }
        public string NombreProveedor { get; set; }
        public double PrecioUnidad { get; set; }
        public int IdPromocion { get; set; }
        public string Promocion { get; set; }
        public double Descuento { get; set; }
    }

    public class ProductosAgrupadosDTO
    {
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        public string? Image_url { get; set; }
        public string NombreCategoria { get; set; }
        public string NombreProveedor { get; set; }
        public List<ProductoListarDTO> Productos { get; set; }
    }
}
