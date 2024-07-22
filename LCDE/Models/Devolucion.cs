using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class Devolucion
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe ingresar una cantidad válida")]
        public int Cantidad { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Motivo { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar un producto")]
        [Display(Name = "Producto")]
        public int Id_producto { get; set; }
        public DateTime? Fecha { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Tipo de devolución")]
        public string Tipo_devolucion { get; set; }
    }
}


