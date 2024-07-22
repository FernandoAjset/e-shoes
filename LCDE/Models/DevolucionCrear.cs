using Microsoft.AspNetCore.Mvc.Rendering;

namespace LCDE.Models
{
    public class DevolucionCrear : Devolucion
    {
        public string? NombreProducto { get; set; }
        public List<SelectListItem>? Productos { get; set; }
    }
}
