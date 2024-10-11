using LCDE.Models;
using LCDE.Models.Enums;
using LCDE.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace LCDE.Controllers;

/// <summary>
/// Controlador con acciones relacionadas a la entidad de Cliente.
/// </summary>
[Authorize(Policy = "AdminOrVendedorPolicy")]
public class VentasController : Controller
{
    private readonly RepositorioProductos repositorioProductos;
    private readonly RepositorioCategorias repositorioCategorias;
    private readonly RepositorioProveedores repositorioProveedores;
    private readonly RepositorioTipoPago repositorioTipoPago;
    private readonly IRepositorioCliente repositotioClientes;
    private readonly RepositorioVentas repositorioVentas;
    private readonly ReportesServicio reportesServicio;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogService logService;

    /// <summary>
    /// Constructor de clase.
    /// </summary>
    /// <param name="repositorioProveedores">Inyección del servicio de repositorio de clientes, que realiza el acceso a la base de datos.</param>
    public VentasController(
        RepositorioProductos repositorioProductos,
        RepositorioCategorias repositorioCategorias,
        RepositorioProveedores repositorioProveedores,
        RepositorioTipoPago repositorioTipoPago,
        IRepositorioCliente repositotioClientes,
        RepositorioVentas repositorioVentas,
        ReportesServicio reportesServicio,
        IHttpContextAccessor httpContextAccessor,
        ILogService logService
        )
    {
        this.repositorioProductos = repositorioProductos;
        this.repositorioCategorias = repositorioCategorias;
        this.repositorioProveedores = repositorioProveedores;
        this.repositorioTipoPago = repositorioTipoPago;
        this.repositotioClientes = repositotioClientes;
        this.repositorioVentas = repositorioVentas;
        this.reportesServicio = reportesServicio;
        this.httpContextAccessor = httpContextAccessor;
        this.logService = logService;
    }
    /// <summary>
    /// Enpoint para devolver la vista de listado de clientes.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    /// <summary>
    /// Endpoint para devolver la vista de creación de un nuevo cliente.
    /// </summary>
    /// <returns>La vista del formulario de creación.</returns>
    [HttpGet]
    public async Task<IActionResult> Crear()
    {

        var modelo = new ProductoCreacionDTO();
        // Obtener listado de categorías
        var categorias = await repositorioCategorias.ObtenerTodosCategorias();
        var resultadoCategorias = categorias
                .Select(x => new SelectListItem(x.Nombre, x.Id.ToString())).ToList();
        var opcionPorDefectoCategoria = new SelectListItem("-- Seleccione una categoría --", "0", true);
        resultadoCategorias.Insert(0, opcionPorDefectoCategoria);
        modelo.Categorias = resultadoCategorias;

        // Obtener listado de proveedores
        var proveedores = await repositorioProveedores.ObtenerTodosProveedores();
        var resultadoProveedores = proveedores
                .Select(x => new SelectListItem(x.Nombre, x.Id.ToString())).ToList();
        var opcionPorDefectoProveedor = new SelectListItem("-- Seleccione una categoría --", "0", true);
        resultadoProveedores.Insert(0, opcionPorDefectoProveedor);
        modelo.Proveedores = resultadoProveedores;

        // Obtener la configuración de cultura desde la ViewBag
        CultureInfo culture = new("en-US");
        culture.NumberFormat.NumberDecimalSeparator = ".";
        // Pasar la configuración de cultura a la vista
        ViewBag.Culture = culture;
        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarVenta([FromBody] VentaViewModel modelo)
    {
        try
        {
            modelo.EncabezadoFactura.EstadoFacturaId = (int)FacturaEstadoEnum.Pagada;
            int IdNuevaFactura = await repositorioVentas.CrearVenta(modelo);
            if (IdNuevaFactura > 0)
            {
                var facturaUrl = await reportesServicio.CrearFactura(IdNuevaFactura);
                await repositorioVentas.AgregarUrlFactura(facturaUrl, IdNuevaFactura);
                return StatusCode(StatusCodes.Status200OK, new { filePath = facturaUrl });
            }
            else
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Error al crear la venta.");
            }
        }
        catch (Exception ex)
        {
            var log = new Log()
            {
                Type = "Error",
                Message = ex.Message,
                StackTrace = ex.StackTrace ?? "",
                Date = DateTime.Now
            };
            logService.Log(log);
            return StatusCode(StatusCodes.Status422UnprocessableEntity, ex);
        }
    }

    /// <summary>
    /// Enpoint para mostrar un registro existente y modificar sus propiedades.
    /// </summary>
    /// <param name="Id">Id del registro que se va a editar</param>
    /// <returns>Una acción de navegación a la edición del registro o la página de error.</returns>
    [HttpGet]
    public async Task<ActionResult> Editar(int Id)
    {
        try
        {
            ProductoCreacionDTO producto = await repositorioProductos.ObtenerProducto(Id);
            if (producto == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            // Obtener listado de categorías
            var categorias = await repositorioCategorias.ObtenerTodosCategorias();
            var resultadoCategorias = categorias
                    .Select(x => new SelectListItem(x.Nombre, x.Id.ToString())).ToList();
            if (producto.Id_Categoria == 0)
            {
                var opcionPorDefectoCategoria = new SelectListItem("-- Seleccione una categoría --", "0", true);
                resultadoCategorias.Insert(0, opcionPorDefectoCategoria);
            }
            producto.Categorias = resultadoCategorias;

            // Obtener listado de proveedores
            var proveedores = await repositorioProveedores.ObtenerTodosProveedores();
            var resultadoProveedores = proveedores
                    .Select(x => new SelectListItem(x.Nombre, x.Id.ToString())).ToList();
            if (producto.Id_Proveedor == 0)
            {
                var opcionPorDefectoProveedor = new SelectListItem("-- Seleccione una categoría --", "0", true);
                resultadoProveedores.Insert(0, opcionPorDefectoProveedor);
            }
            producto.Proveedores = resultadoProveedores;

            // Obtener la configuración de cultura desde la ViewBag
            CultureInfo culture = new CultureInfo("en-US");
            culture.NumberFormat.NumberDecimalSeparator = ".";
            // Pasar la configuración de cultura a la vista
            ViewBag.Culture = culture;
            return View(producto);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Home");
        }
    }

    // POST: ClientesController1/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Editar(ProductoCreacionDTO producto)
    {
        try
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            decimal cantidad;
            string valorIngresado = producto.PrecioUnidadString; // Valor ingresado en el campo de texto

            if (decimal.TryParse(valorIngresado, NumberStyles.Number, culture, out cantidad))
            {
                producto.PrecioUnidad = cantidad;
            }
            // Validación de las validaciones del modelo.
            if (!ModelState.IsValid && producto.Id != 0)
            {
                List<string> errores = new();
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    errores.Add(error.ErrorMessage);
                }
                ViewData["Errores"] = errores;

                return View(producto);
            }
            bool codigoResult = await repositorioProductos.ModificarProducto(producto);
            if (codigoResult)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
        catch
        {
            return RedirectToAction("Error", "Home");
        }
    }

    // GET: ClientesController1/Delete/5
    [HttpGet]
    public async Task<IActionResult> Borrar(int id)
    {
        var producto = await repositorioProductos.ObtenerProducto(id);

        if (producto is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        return View(producto);
    }

    [HttpPost]
    public async Task<IActionResult> BorrarProducto(int id)
    {
        var producto = await repositorioProductos.ObtenerProducto(id);
        if (producto is null)
        {
            return RedirectToAction("NoEncontrado", "Home");
        }

        if (await repositorioProductos.BorrarProducto(id))
        {
            return Ok(new { success = true });
        }
        return BadRequest();
    }

    [HttpGet]
    public async Task<IActionResult> ProductoExiste(int Id, string Detalle)
    {
        Producto respuesta = await repositorioProductos.ObtenerProductoPorNombre(Id, Detalle);
        if (respuesta is not null)
        {
            return Json($"Ya existe un producto con nombre {Detalle}");
        }
        return Json(true);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerProductos()
    {
        IEnumerable<ProductoListarDTO> productos = await repositorioProductos.ObtenerTodosProductos();
        if (productos is not null)
        {
            foreach (var product in productos)
            {
                product.Detalle = $" {product.Nombre} - Talla: {product.talla}";
            }

            return StatusCode(StatusCodes.Status200OK, productos);
        }

        return RedirectToAction("Error", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerClientes()
    {
        IEnumerable<Cliente> clientes = await repositotioClientes.ObtenerTodosClientes();
        if (clientes is not null)
        {
            return StatusCode(StatusCodes.Status200OK, clientes);
        }
        return RedirectToAction("Error", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTiposPago()
    {
        IEnumerable<TipoPago> tipos = await repositorioTipoPago.ObtenerTodosTipoPago();
        if (tipos is not null)
        {
            return StatusCode(StatusCodes.Status200OK, tipos.Where(t => t.Tipo.ToUpper().Equals("EFECTIVO")));
        }
        return RedirectToAction("Error", "Home");
    }

    private System.Net.IPAddress? GetIPv4Address(System.Net.IPAddress? ipAddress)
    {
        if (ipAddress == null)
            return null;

        if (System.Net.IPAddress.IsLoopback(ipAddress))
        {
            // Obtener todas las direcciones IP disponibles
            var hostAddresses = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());

            // Filtrar y obtener la dirección IPv4
            ipAddress = hostAddresses.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }

        return ipAddress;
    }
}
