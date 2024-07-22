namespace LCDE.Models
{
    public class DetalleFactura
    {
        public int Id { get; set; }
        public decimal Subtotal { get; set; }

        public int Cantidad { get; set; }
        public int IdProducto { get; set; }
        public int IdEncabezadoFactura { get; set; }
        public decimal Descuento { get; set; }

    }
}
