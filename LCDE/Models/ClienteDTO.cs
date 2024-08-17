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
        [Required(ErrorMessage = "El campo {0} es requerido, puede colocar CF")]
        [RegularExpression(@"^(?!0+$)[a-zA-Z0-9-]+$", ErrorMessage = "El campo solo puede contener letras, números y guiones, y no puede ser solo ceros")]
        public string NIT { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Solo se permiten letras.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "El formato del telefono no es válido")]
        [MinLength(8, ErrorMessage = "El campo debe ser de 8 caracteres")]
        [MaxLength(8, ErrorMessage = "El campo debe ser de 8 caracteres")]
        [RegularExpression(@"^(?!0+$)\d{8}$", ErrorMessage = "El campo no puede contener solo ceros")]
        public string? Telefono { get; set; }
    }

    public class DatosUsuario
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Solo se permiten letras.")]
        public string Nombre_usuario { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido")]
        public string Correo { get; set; }
        [MinLength(8, ErrorMessage = "El campo debe tener al menos 8 caracteres")]
        [Display(Name = "Contraseña")]
        public string? Contrasennia { get; set; }
    }
}