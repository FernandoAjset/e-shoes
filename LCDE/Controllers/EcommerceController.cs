using LCDE.Models;
using LCDE.Models.Enums;
using LCDE.Models.Transaction;
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
        private readonly ReportesServicio reportesServicio;
        private readonly IEmailService emailService;
        private readonly IFileRepository fileRepository;
        private readonly IConfiguration configuration;
        private readonly UserManager<Usuario> userManager;

        public EcommerceController(
            IConfiguration configuration,
            UserManager<Usuario> userManager,
            IRepositorioCliente repositorioClientes,
            ISesionServicio sesionServicio,
            IRepositorioUsuarios repositorioUsuarios,
            RepositorioCategorias repositorioCategorias,
            RepositorioProductos repositorioProductos,
            RepositorioVentas repositorioVentas,
            ReportesServicio reportesServicio,
            IEmailService emailService,
            IFileRepository fileRepository
            )
        {

            this.repositotioClientes = repositorioClientes;
            this.sesionServicio = sesionServicio;
            this.repositorioUsuarios = repositorioUsuarios;
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioProductos = repositorioProductos;
            this.repositorioVentas = repositorioVentas;
            this.reportesServicio = reportesServicio;
            this.emailService = emailService;
            this.fileRepository = fileRepository;
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

        [HttpGet]
        public async Task<IActionResult> HistorialCompras()
        {
            try
            {
                var idUsuario = sesionServicio.ObtenerIdUsuarioSesion();
                Usuario usuario = await repositorioUsuarios.BuscarUsuarioId(idUsuario);

                Cliente cliente = await repositotioClientes.ObtenerClientePorIdUsuario(usuario.Id);

                if (cliente is null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }

                var facturas = await repositorioVentas.ObtenerFacturasPorCliente(cliente.Id);
                return View(facturas);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
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
            try
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
                    var facturaUrl = await reportesServicio.CrearFactura(facturaId);
                    await repositorioVentas.AgregarUrlFactura(facturaUrl, facturaId);

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
            catch
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
                var userName = configuration.GetValue<string>("Paypal:USER");
                var passwd = configuration.GetValue<string>("Paypal:PASSWORD");

                client.BaseAddress = new Uri("https://api-m.sandbox.paypal.com");

                var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                var orden = new PaypalOrder()
                {
                    intent = "CAPTURE",
                    purchase_units = new List<Models.PurchaseUnit>() {
                    new() {
                        amount = new Models.Amount() {
                            currency_code = "USD",
                            value = pagoData.Precio
                        },
                        description = pagoData.Descripcion
                    }
                },
                    application_context = new ApplicationContext()
                    {
                        brand_name = "Eshoes",
                        landing_page = "NO_PREFERENCE",
                        user_action = "PAY_NOW", // Accion para que paypal muestre el monto de pago
                        return_url = $"{configuration.GetValue<string>("AppUrl")}/Ecommerce/ConfirmarPagoPayPal", // cuando se aprovo la solicitud del cobro
                        cancel_url = $"{configuration.GetValue<string>("AppUrl")}/Ecommerce/HistorialCompras" // cuando cancela la operacion
                    }
                };

                var json = JsonConvert.SerializeObject(orden);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/v2/checkout/orders", data);

                status = response.IsSuccessStatusCode;

                if (status)
                {
                    respuesta = await response.Content.ReadAsStringAsync();
                    // Obtener el token de la respuesta
                    var objeto = JsonConvert.DeserializeObject<PaypalOrderResponse>(respuesta);
                    if (objeto == null)
                    {
                        return Json(new { status = false, respuesta = "No se pudo completar el pago, intente más tarde" });
                    }
                    // Actualizar el token de pago en la factura
                    await repositorioVentas.AgregarInfoPagoFactura(pagoData.IdOrden, objeto);
                }
            }

            return Json(new { status, respuesta });
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmarPagoPayPal([FromQuery] string token, [FromQuery] string PayerID)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(PayerID))
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "No se pudo completar el pago, intente más tarde";
                return RedirectToAction("Error", "Home");
            }

            bool status = false;

            using (var client = new HttpClient())
            {
                var userName = configuration.GetValue<string>("Paypal:USER");
                var passwd = configuration.GetValue<string>("Paypal:PASSWORD");

                client.BaseAddress = new Uri("https://api-m.sandbox.paypal.com");

                var authToken = Encoding.ASCII.GetBytes($"{userName}:{passwd}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                var data = new StringContent("{}", Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"/v2/checkout/orders/{token}/capture", data);

                status = response.IsSuccessStatusCode;

                ViewData["Status"] = status;
                if (status)
                {
                    // Actualizar estado factura a pagado
                    await repositorioVentas.ActualizarEstadoVentaPagado(token);
                    try
                    {
                        // Enviar correo
                        var factura = await repositorioVentas.ObtenerFacturaPorTokenPago(token);
                        // Convertir a archivo adjunto basado en el url de la factura
                        var facturaUrl = factura.Url;
                        var attachment = await fileRepository.DownloadFileAsFormFileAsync(facturaUrl, $"Factura_{factura.Id}.pdf");
                        List<IFormFile> attachments = new();
                        if (attachment != null)
                        {
                            attachments.Add(attachment);
                        }

                        await emailService.SendEmailAsync(factura.CorreoCliente!, "Pago realizado",
                                                          @$"Su pago ha sido realizado exitosamente. 
                                      A continuación se adjunta su factura No.{factura.Id}.", attachments);
                    }
                    catch
                    {
                        TempData["ToastType"] = "error";
                        TempData["ToastMessage"] = "El pago se realizó correctamente, pero no se pudo enviar la copia de su factura, por favor contacte con la tienda.";
                    }

                    var jsonRespuesta = response.Content.ReadAsStringAsync().Result;

                    PaypalTransaction objeto = JsonConvert.DeserializeObject<PaypalTransaction>(jsonRespuesta);

                    ViewData["IdTransaccion"] = objeto.purchase_units[0].payments.captures[0].id;

                    TempData["ToastType"] = "success";
                    TempData["ToastMessage"] = "Pago realizado exitosamente";
                }
                else
                {
                    TempData["ToastType"] = "error";
                    TempData["ToastMessage"] = "No se pudo completar el pago, intente más tarde";
                }
            }

            return RedirectToAction("Home");
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

        public async Task<IActionResult> DescargarFactura(int id)
        {
            try
            {
                // Lógica para obtener la factura por id y generar el archivo
                var factura = await repositorioVentas.ObtenerEncabezadoFacturaPorId(id);
                if (factura == null)
                {
                    return NotFound();
                }
                // Convertir a archivo adjunto basado en el url de la factura
                var facturaUrl = factura.Url;
                var attachment = await fileRepository.DownloadFileAsFormFileAsync(facturaUrl, $"Factura_{factura.Id}.pdf");
                if (attachment == null)
                {
                    return NotFound();
                }
                return File(attachment.OpenReadStream(), "application/pdf", $"Factura_{factura.Id}.pdf");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}