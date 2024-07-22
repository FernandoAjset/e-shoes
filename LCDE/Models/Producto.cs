using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class Producto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Nombre producto")]
        [Remote(action: "ProductoExiste", controller: "Productos",
            AdditionalFields = nameof(Id))]

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
