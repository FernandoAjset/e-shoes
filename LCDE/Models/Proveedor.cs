using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int Telefono { get; set; }
        public string? Correo { get; set; }
        [Remote(action: "ProveedorExiste", controller: "Proveedores",
    AdditionalFields = nameof(Id))]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nit { get; set; }
    }
}
