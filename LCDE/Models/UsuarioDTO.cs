using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class UsuarioDTO 
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(70, ErrorMessage = "El campo no puede exceder 70 caracteres")]
        public string Nombre_usuario { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido")]
        public string Correo { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]

        public int Id_Role { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}