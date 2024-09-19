using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Display(Name = "Imagen del producto")]
        [Url(ErrorMessage = "Debe ser una URL válida.")]
        public string? Image_url { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Nombre producto")]
        [RegularExpression(@"^(?!\d)[A-Za-z0-9\s]+(?<!\s)$", ErrorMessage = "El nombre no es válido.")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Descripcion del Producto")]
        [Remote(action: "ProductoExiste", controller: "Productos",
            AdditionalFields = nameof(Id))]
        [RegularExpression(@"^(?!\d)[A-Za-z0-9\s]+(?<!\s)$", ErrorMessage = "El nombre no es válido.")]
        public string Detalle { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Existencia inicial")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe ingresar una cantidad válida")]
        public int Existencia { get; set; }
        [Display(Name = "Categoria")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una categoría")]
        public int Id_Categoria { get; set; }
        [Display(Name = "Proveedor")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar un proveedor")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? Id_Proveedor { get; set; }
        public int? IdPrecio { get; set; }
        public int? IdPromocion { get; set; }
    }
}
