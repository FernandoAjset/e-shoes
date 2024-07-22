namespace LCDE.Models
{
    public class Precio
    {
            public int Id { get; set; }
            public int IdProducto { get; set; }
            public decimal PrecioUnidad { get; set; }
            public decimal? DescuentoProducto { get; set; }
            public DateTime? FechaModificacionPrecio { get; set; }
        }
}
