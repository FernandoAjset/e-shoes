using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class UsuarioCrearDTO : UsuarioDTO
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(8, ErrorMessage = "El campo debe tener al menos 8 caracteres")]
        public string Contrasennia { get; set; }
    }
}