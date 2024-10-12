using Microsoft.AspNetCore.Mvc.Rendering;

namespace LCDE.Models
{
    public class ProductoFiltroDTO
    {
        public int CategoriaId { get; set; }

        public string? NombreProducto { get; set; }

        public decimal PrecioMin { get; set; }

        public decimal PrecioMax { get; set; }

        public float Talla { get; set; }

        public IEnumerable<SelectListItem>? Tallas { get; set; }

        public List<Categoria>? Categorias { get; set; }
        public List<ProductoListarDTO>? productosListarDTO { get; set; }

    }
}