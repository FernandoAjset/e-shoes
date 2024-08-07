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
        [DataType(DataType.PhoneNumber)]
        public int Telefono { get; set; }
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido")]
        public string? Correo { get; set; }
    }

    public class CrearUusarioCliente 
    {
        public CrearClienteDTo informacionCliente { get; set; }
        public UsuarioCrearDTO informacionUsuario { get; set; }
    }
}
