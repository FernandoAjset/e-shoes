using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace LCDE.Controllers
{
    [Authorize(Policy = "ClientePolicy")]
    public class EcommerceController : Controller
    {
        private readonly IRepositorioCliente repositotioClientes;
        private readonly ISesionServicio sesionServicio;
        private readonly IRepositorioUsuarios repositorioUsuarios;
        private readonly RepositorioCategorias repositorioCategorias;
        private readonly RepositorioProductos repositorioProductos;
        private readonly UserManager<Usuario> userManager;

        public EcommerceController(UserManager<Usuario> userManager,
            IRepositorioCliente pepe, //quien chingados le pone pepe a una interfaz no mameen xD
            ISesionServicio sesionServicio,
            IRepositorioUsuarios repositorioUsuarios,
            RepositorioCategorias repositorioCategorias,
            RepositorioProductos repositorioProductos
            )
        {

            this.repositotioClientes = pepe;
            this.sesionServicio = sesionServicio;
            this.repositorioUsuarios = repositorioUsuarios;
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioProductos = repositorioProductos;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult ResumenCarrito()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResumenCarrito([FromBody] List<CarritoItemDTO> carrito)
        {
            if (carrito == null || !carrito.Any())
            {
                return PartialView("_ResumenCarritoPartial", new List<CarritoItemDTO>());
            }

            // Obtener los IDs de los productos en el carrito
            var idsProductos = carrito.Select(c => c.IdProducto).ToList();

            // Obtener los detalles de los productos desde el repositorio
            var productos = await repositorioProductos.ObtenerDetallesProductos(idsProductos);

            // Combinar la información del carrito con los detalles de los productos
            var carritoDetalles = carrito.Join(productos, c => c.IdProducto, p => p.Id, (c, p) => new CarritoItemDTO
            {
                IdProducto = c.IdProducto,
                NombreProducto = p.Nombre,
                Cantidad = c.Cantidad,
                PrecioUnidad = p.PrecioUnidad
            }).ToList();

            return PartialView("_ResumenCarritoPartial", carritoDetalles);
        }

        public async Task<IActionResult> Home()
        {
            var model = new EcommerceHomeViewModel();
            model.Categorias = await repositorioCategorias.ObtenerTodosCategorias();
            return View(model);
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
                    Telefono = cliente.Telefono.ToString(),
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

                var ClientExist = await repositotioClientes.ObtenerClientePorIdUsuario(ClienteUsuario.Usuario.Id);

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
                            if (!addPasswordResult.Succeeded)
                            {
                                ModelState.AddModelError(string.Empty, "Error al cambiar contraseña.");
                                return View(ClienteUsuario);
                            }

                            // Mandar notificación por cambio de contraseña
                            await repositorioUsuarios.NotificacionContrasenia(ClienteUsuario.Usuario.Correo);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Error al cambiar contraseña.");
                            return View(ClienteUsuario);
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
                ClientExist.Telefono = int.Parse(ClienteUsuario.Cliente.Telefono ?? "0");
                ClientExist.Correo = ClienteUsuario.Usuario.Correo;
                await repositotioClientes.ModificarCliente(ClientExist);

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
