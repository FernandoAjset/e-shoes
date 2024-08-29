namespace LCDE.Models
{
    public class CarritoItemDTO
    {
        public int IdProducto { get; set; }
        public string ImageUrl { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnidad { get; set; }
        public double Total => Cantidad * PrecioUnidad;
    }
}