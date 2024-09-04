using LCDE.Validaciones;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class ProductoCreacionDTO : Producto
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Precio de unidad")]
        [Range(1, maximum: double.MaxValue, ErrorMessage = "Debe ingresar una cantidad válida")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo {0} debe ser un número válido.")]
        public string PrecioUnidadString { get; set; }

        public decimal PrecioUnidad { get; set; }

        [Display(Name = "Seleccionar Imagen")]
        [DataType(DataType.Upload)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile? Imagen { get; set; }

        public IEnumerable<SelectListItem>? Categorias { get; set; }
        public IEnumerable<SelectListItem>? Proveedores { get; set; }

    }
}
