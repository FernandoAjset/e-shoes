using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LCDE.Models
{
    public class ClienteUsuarioEditarDTO
    {
        public int Id { get; set; }
        public int Id_usuario { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Remote(action: "ClienteExiste", controller: "Clientes",
            AdditionalFields = nameof(Id))]
        public string NIT { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.PhoneNumber)]
        public int Telefono { get; set; }

        public UsuarioActualizarDTO UsuarioActualizar { get; set; }

    }
}
