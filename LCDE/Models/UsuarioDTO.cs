using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class UsuarioDTO 
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(70, ErrorMessage = "El campo no puede exceder 70 caracteres")]
        [Display(Name = "Nombre de usuario")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Solo se permiten letras.")]
        public string Nombre_usuario { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido")]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Rol")]

        public int Id_Role { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
} 




















//[ValidarLetras(ErrorMessage = "El campo solo puede contener letras de la A a la Z")]