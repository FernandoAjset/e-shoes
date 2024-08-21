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

        //Obtener detalle de productos
        public int Id { get; set; }
        public string Detalle { get; set; }
        public int Existencia { get; set; }
        public string Descripcion { get; set; }

    }
}
