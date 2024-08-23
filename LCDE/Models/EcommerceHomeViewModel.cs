namespace LCDE.Models
{
    public class EcommerceHomeViewModel
    {
        public IEnumerable<Categoria> Categorias { get; set; }
        public IEnumerable<ProductoListarDTO> Productos { get; set; }
    }
}