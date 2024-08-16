using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class CrearClienteDTo
    {

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "El formato del telefono no es válido")]
        [MinLength(8, ErrorMessage = "El campo debe ser de 8 caracteres")]
        [MaxLength(8, ErrorMessage = "El campo debe ser de 8 caracteres")]
        public string? Telefono { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido, puede colocar CF")]
        public string? Nit { get; set; }

    }

    public class CrearUsarioCliente
    {
        public CrearClienteDTo informacionCliente { get; set; }
        public RegistroUsuarioCliente informacionUsuario { get; set; }
    }

    public class RegistroUsuarioCliente
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
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MinLength(8, ErrorMessage = "El campo debe tener al menos 8 caracteres")]
        [Display(Name = "Contraseña")]
        public string Contrasennia { get; set; }

    }
}
