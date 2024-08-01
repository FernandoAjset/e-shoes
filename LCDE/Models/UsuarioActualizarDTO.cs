using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class UsuarioActualizarDTO: UsuarioDTO
    {
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "El campo debe tener al menos 8 caracteres")]
        public string? Contrasennia { get; set; }
    }
}
