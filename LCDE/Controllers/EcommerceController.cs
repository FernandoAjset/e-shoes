using LCDE.Models;
using LCDE.Models.Enums;
using LCDE.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

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
        private readonly RepositorioVentas repositorioVentas;
        private readonly IConfiguration configuration;
        private readonly UserManager<Usuario> userManager;

        public EcommerceController(
            IConfiguration configuration,
            UserManager<Usuario> userManager,
            IRepositorioCliente pepe, //quien chingados le pone pepe a una interfaz no mameen xD
            ISesionServicio sesionServicio,
            IRepositorioUsuarios repositorioUsuarios,
            RepositorioCategorias repositorioCategorias,
            RepositorioProductos repositorioProductos,
            RepositorioVentas repositorioVentas
            )
        {

            this.repositotioClientes = pepe;
            this.sesionServicio = sesionServicio;
            this.repositorioUsuarios = repositorioUsuarios;
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioProductos = repositorioProductos;
            this.repositorioVentas = repositorioVentas;
            this.configuration = configuration;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult ResumenCarrito()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmarOrden()
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
            var carritoDetalles = carrito.Join(productos, c => c.IdProducto, p => p.IdProducto, (c, p) => new CarritoItemDTO
            {
                ImageUrl = p.ImageUrl,
                IdProducto = c.IdProducto,
                NombreProducto = p.NombreProducto,
                Cantidad = c.Cantidad,
                PrecioUnidad = p.PrecioUnidad,
                Existencia = p.Existencia
            }).ToList();

            return PartialView("_ResumenCarritoPartial", carritoDetalles);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarOrden([FromBody] List<CarritoItemDTO> carrito)
        {
            if (carrito == null || !carrito.Any())
            {
                return PartialView("_DetalleOrdenPartial", new List<CarritoItemDTO>());
            }

            // Obtener los IDs de los productos en el carrito
            var idsProductos = carrito.Select(c => c.IdProducto).ToList();

            // Obtener los detalles de los productos desde el repositorio
            var productos = await repositorioProductos.ObtenerDetallesProductos(idsProductos);

            // Combinar la información del carrito con los detalles de los productos
            var carritoDetalles = carrito.Join(productos, c => c.IdProducto, p => p.IdProducto, (c, p) => new CarritoItemDTO
            {
                ImageUrl = p.ImageUrl,
                IdProducto = c.IdProducto,
                NombreProducto = p.NombreProducto,
                Cantidad = c.Cantidad,
                PrecioUnidad = p.PrecioUnidad,
                Existencia = p.Existencia
            }).ToList();

            // Obtener datos del cliente
            var idUsuario = sesionServicio.ObtenerIdUsuarioSesion();
            Usuario usuario = await repositorioUsuarios.BuscarUsuarioId(idUsuario);

            Cliente cliente = await repositotioClientes.ObtenerClientePorIdUsuario(usuario.Id);
            var data = new ConfirmarOrdenDTO()
            {
                ClienteInfo = cliente,
                DetallesCarrito = carritoDetalles
            };
            return PartialView("_DetalleOrdenPartial", data);
        }

        [HttpPost]
        public async Task<IActionResult> CrearOrden([FromBody] ConfirmarOrdenDTO nuevaOrden)
        {
            if (nuevaOrden == null)
            {
                return Json(new { success = false, message = "Datos de la orden no válidos" });
            }

            // Crear la orden en estado pendiente de pago
            var venta = new VentaViewModel
            {
                EncabezadoFactura = new EncabezadoFactura
                {
                    Serie = "A",
                    Fecha = DateTime.Now,
                    IdTipoPago = (int)TipoPagoEnum.PayPal,
                    IdCliente = nuevaOrden.ClienteInfo.Id,
                    EstadoFacturaId = (int)FacturaEstadoEnum.PendientePago
                },
                DetallesFactura = nuevaOrden.DetallesCarrito.Select(item => new DetalleFactura
                {
                    Subtotal = 0,
                    Cantidad = item.Cantidad,
                    IdProducto = item.IdProducto,
                    Descuento = 0
                }).ToList()
            };

            int facturaId = await repositorioVentas.CrearVenta(venta);

            if (facturaId > 0)
            {
                // Obtener detalles de la factura y sumar el total
                var detalles = await repositorioVentas.ObtenerDetallesFactura(facturaId);
                decimal precioTotal = detalles.Sum(d => d.Subtotal);

                // Descripción para PayPal
                var productoDescripcion = $"Pago de factura No.{facturaId}";

                return Json(new { success = true, precio = precioTotal, descripcion = productoDescripcion, idOrden = facturaId });
            }
            else
            {
                return Json(new { success = false, message = "Error al crear la orden" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Paypal([FromBody] PaypalDTO pagoData)
        {
            bool status = false;
            string respuesta = string.Empty;

            using (var client = new HttpClient())
            {
                var userName = configuration.GetValue<string>("Paypal:User");
                var passwd = configuration.GetValue<string>("Paypal:Password");

                client.BaseAddress = new Uri("https://api-m.sandbox.paypal.com");

                var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                var orden = new PaypalOrder()
                {
                    intent = "CAPTURE",
                    purchase_units = new List<PurchaseUnit>() {
                    new PurchaseUnit() {
                        amount = new Amount() {
                            currency_code = "USD",
                            value = pagoData.Precio
                        },
                        description = pagoData.Descripcion
                    }
                },
                    application_context = new ApplicationContext()
                    {
                        brand_name = "E-shoes",
                        landing_page = "NO_PREFERENCE",
                        user_action = "PAY_NOW", // Accion para que paypal muestre el monto de pago
                        return_url = $"{configuration.GetValue<string>("AppUrl")}/Ecommerce/Orden?idOrden={pagoData.IdOrden}", // cuando se aprovo la solicitud del cobro
                        cancel_url = $"{configuration.GetValue<string>("AppUrl")}/Ecommerce/Orden?idOrden={pagoData.IdOrden}" // cuando cancela la operacion
                    }
                };

                var json = JsonConvert.SerializeObject(orden);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/v2/checkout/orders", data);

                status = response.IsSuccessStatusCode;

                if (status)
                {
                    respuesta = await response.Content.ReadAsStringAsync();
                }
            }

            return Json(new { status = status, respuesta = respuesta });
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
                    Nit = cliente.Nit,
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
                ClientExist.Nit = ClienteUsuario.Cliente.Nit;
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
