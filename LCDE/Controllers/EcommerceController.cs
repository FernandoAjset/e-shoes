using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LCDE.Controllers
{
    [Authorize(Policy = "ClientePolicy")]
    public class EcommerceController : Controller
    {
        private readonly IRepositorioCliente repositotioClientes;
        private readonly ISesionServicio sesionServicio;
        private readonly IRepositorioUsuarios repositorioUsuarios;
        private readonly UserManager<Usuario> userManager;
        public EcommerceController(UserManager<Usuario> userManager,
            IRepositorioCliente pepe,
            ISesionServicio sesionServicio,
            IRepositorioUsuarios repositorioUsuarios)
        {

            this.repositotioClientes = pepe;
            this.sesionServicio = sesionServicio;
            this.repositorioUsuarios = repositorioUsuarios;

        }


        public IActionResult Home()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfiguracionPerfil()
        {
            try
            {
                var idUsuario = sesionServicio.ObtenerIdUsuarioSesion();
                Usuario usuario = await repositorioUsuarios.BuscarUsuarioId(idUsuario);

                Cliente cliente = await repositotioClientes.ObtenerClientePorIdUsuario(usuario.Id);

                ClienteDTO clienteDTO = new ClienteDTO();
                if (usuario is null || cliente is null || cliente.Id == 0)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }


                clienteDTO.Cliente = new DatosCliente()
                {
                    NIT = cliente.NIT,
                    Nombre = cliente.Nombre,
                    Direccion = cliente.Direccion,
                    Telefono = cliente.Telefono,
                    Correo = cliente.Correo,
                };

                clienteDTO.Usuario = new DatosUsuario()
                {
                    Id = usuario.Id,
                    Nombre_usuario = usuario.Nombre_usuario,
                    Correo = usuario.Correo,
                };

                return View(clienteDTO);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfiguracionPerfil(ClienteDTO ClienteUsuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(ClienteUsuario);
                }

                var ClientExist = await repositotioClientes.ObtenerCliente(ClienteUsuario.Usuario.Id);

                if (ClientExist == null)
                {
                    ModelState.AddModelError(string.Empty, "Cliente no existe.");
                    return RedirectToAction("NoEncontrado", "Home");
                }

                var usuarioExist = await userManager.FindByIdAsync(ClientExist.Id_usuario.ToString());
                if (usuarioExist == null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }

                usuarioExist.Nombre_usuario = ClienteUsuario.Usuario.Nombre_usuario;
                usuarioExist.Correo = ClienteUsuario.Usuario.Correo;


                bool codigoResult = await repositorioUsuarios.EditarUsuario(usuarioExist);

                if (codigoResult)
                {
                    if (!string.IsNullOrEmpty(ClienteUsuario.Usuario.Contrasennia))
                    {
                        var removePasswordResult = await userManager
                                                        .RemovePasswordAsync(usuarioExist);
                        if (removePasswordResult.Succeeded)
                        {
                            var addPasswordResult = await userManager
                                .AddPasswordAsync(usuarioExist, ClienteUsuario.Usuario.Contrasennia);
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }

                // Actualizar cliente
                ClientExist.NIT = ClienteUsuario.Cliente.NIT;
                ClientExist.Nombre = ClienteUsuario.Cliente.Nombre;
                ClientExist.Direccion = ClienteUsuario.Cliente.Direccion;
                ClientExist.Telefono = ClienteUsuario.Cliente.Telefono ?? 0;
                ClientExist.Correo = ClienteUsuario.Usuario.Correo;

                return RedirectToAction("Home");
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerRoles()
        {
            return await repositorioUsuarios.ObtenerRoles();
        }

    }
}
