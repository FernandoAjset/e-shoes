using Microsoft.AspNetCore.Mvc.Rendering;

namespace LCDE.Models
{
    public class UsuarioDTO 
    {
        public int? Id { get; set; }
        public string Nombre_usuario { get; set; }
        public string Contrasennia { get; set; }
        public string Correo { get; set; }
        public int IdRole { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}