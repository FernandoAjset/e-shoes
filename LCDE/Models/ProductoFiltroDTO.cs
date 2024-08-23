namespace LCDE.Models
{
    public class ProductoFiltroDTO
    {
        public int CategoriaId { get; set; }

        public string? NombreProducto { get; set; }

        public decimal PrecioMin { get; set; }

        public decimal PrecioMax { get; set; }

        public List<Categoria>? Categorias { get; set; }
        public List<ProductoListarDTO>? productosListarDTO { get; set; }

        // otra propiedad de tipo lista productos, puede ser lista vacia no nula, de tipo ProductoListarDTO
    }
}