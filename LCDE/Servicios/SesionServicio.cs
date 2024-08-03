using LCDE.Models.Enums;
using System.Security.Claims;

namespace LCDE.Servicios
{
    public class SesionServicio : ISesionServicio
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public SesionServicio(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Rol?> ObtenerRolUsuarioEnumAsync()
        {
            var userRoleClaim = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (userRoleClaim == null)
            {
                throw new UnauthorizedAccessException("Rol de usuario no encontrado.");
            }

            if (Enum.TryParse(typeof(Rol), userRoleClaim.Value, true, out var roleEnum))
            {
                return (Rol?)roleEnum;
            }
            else
            {
                throw new UnauthorizedAccessException("Rol de usuario no válido.");
            }
        }

        public string ObtenerRolUsuario()
        {
            var rolEnum = ObtenerRolUsuarioEnumAsync().GetAwaiter().GetResult();
            return rolEnum.HasValue ? ((int)rolEnum.Value).ToString() : null;
        }

        public string ObtenerNombreRolUsuario()
        {
            var rolEnum = ObtenerRolUsuarioEnumAsync().GetAwaiter().GetResult();
            return rolEnum.HasValue ? rolEnum.Value.ToString() : "Sin Rol";
        }

        public bool EsUsuarioAdmin()
        {
            var rol = ObtenerRolUsuario();
            return rol == ((int)Rol.Admin).ToString();
        }

        public bool EsUsuarioVendedor()
        {
            var rol = ObtenerRolUsuario();
            return rol == ((int)Rol.Vendedor).ToString();
        }

        public bool EsUsuarioCliente()
        {
            var rol = ObtenerRolUsuario();
            return rol == ((int)Rol.Cliente).ToString();
        }
    }

    public interface ISesionServicio
    {
        string ObtenerRolUsuario();
        bool EsUsuarioAdmin();
        bool EsUsuarioVendedor();
        bool EsUsuarioCliente();
        string ObtenerNombreRolUsuario();
    }
}
