using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class ClienteDTO
    {
        public DatosCliente Cliente { get; set; }
        public DatosUsuario Usuario { get; set; }
    }

    public class DatosCliente
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string NIT { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.PhoneNumber)]
        public int? Telefono { get; set; }
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? Correo { get; set; }
    }

    public class DatosUsuario
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre_usuario { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido")]
        public string Correo { get; set; }
        [MinLength(8, ErrorMessage = "El campo debe tener al menos 8 caracteres")]
        [Display(Name = "Contraseña")]
        public string? Contrasennia { get; set; }
    }
}