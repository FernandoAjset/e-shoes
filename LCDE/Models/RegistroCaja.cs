using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class RegistroCaja
    {

        public int Id { get; set; }
        public DateTime? Fecha { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar un turno")]
        [Display(Name = "Turno")]
        public int Turno { get; set; }
        public int? IdCaja { get; set; }
        public int? IdUsuario { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Tipo de registro")]
        public string TipoRegistro { get; set; }
        public decimal? Monto { get; set; }
    }
}
