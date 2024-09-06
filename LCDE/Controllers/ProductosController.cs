using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace LCDE.Controllers
{
    /// <summary>
    /// Controlador con acciones relacionadas a la entidad de Cliente.
    /// </summary>
    [Authorize(Policy = "AdminPolicy")]
    public class ProductosController : Controller
    {
        private readonly RepositorioProductos repositorioProductos;
        private readonly RepositorioCategorias repositorioCategorias;
        private readonly RepositorioProveedores repositorioProveedores;
        private readonly IFileRepository fileRepository;

        private readonly string CarpetaDeImg= "lcde-productos";
        private readonly string rootDefaultImg = "https://schoolcampussur.blob.core.windows.net/lcde-productos/207396005-shoes-vector-thick-line-icon-for-personal-and-commercial-use.jpg";

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="repositorioProveedores">Inyección del servicio de repositorio de clientes, que realiza el acceso a la base de datos.</param>
        public ProductosController(
            RepositorioProductos repositorioProductos,
            RepositorioCategorias repositorioCategorias,
            RepositorioProveedores repositorioProveedores,
            IFileRepository fileRepository
            )
        {
            this.repositorioProductos = repositorioProductos;
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioProveedores = repositorioProveedores;
            this.fileRepository = fileRepository;
        }
        /// <summary>
        /// Enpoint para devolver la vista de listado de clientes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var productos = await repositorioProductos.ObtenerTodosProductos();
            return View(productos);
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
            CultureInfo culture = new CultureInfo("en-US");
            culture.NumberFormat.NumberDecimalSeparator = ".";
            // Pasar la configuración de cultura a la vista
            ViewBag.Culture = culture;
            return View(modelo);
        }

        /// <summary>
        /// Endpoint al realizar el submit del formulario de creación del registro.
        /// </summary>
        /// <param name="cliente">El objeto tipo Cliente que contiene los datos del registro.</param>
        /// <returns>Una acción al index o a la vista de error.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ProductoCreacionDTO producto)
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
                return View(producto);
            }
            //Enviar la imagen a Azure Storage
            if (producto.Imagen != null) producto.Image_url = await fileRepository.AddFile(producto.Imagen, CarpetaDeImg);
            else producto.Image_url = rootDefaultImg;

            // Enviar los datos al repositorio para grabar en base de datos, si se crea el registro se obtiene el nuevo Id.
            if (await repositorioProductos.CrearProducto(producto) > 0)
            {
                // Si la operación es correcta se retorna al index.
                return RedirectToAction("Index");
            }
            else
            {
                // Si ocurre un error se retorna a la vista de error.
                return RedirectToAction("Error", "Home");
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
                    List<string> errores = new List<string>();
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        errores.Add(error.ErrorMessage);
                    }
                    ViewData["Errores"] = errores;

                    return View(producto);
                }

                //Enviar la imagen a Azure Storage
                if (producto.Imagen != null)
                {
                    await fileRepository.DeleteFile(producto.Image_url, CarpetaDeImg);
                    producto.Image_url = await fileRepository.AddFile(producto.Imagen, CarpetaDeImg);
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
                await fileRepository.DeleteFile(producto.Image_url, CarpetaDeImg);
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
        public async Task<IActionResult> ValidarExistenciaSuficiente(int id, int cantidadIngresada)
        {
            Producto respuesta = await repositorioProductos.ObtenerProducto(id);
            if (respuesta is null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Ocurrió un problema al consultar el producto");
            }
            if (cantidadIngresada > respuesta.Existencia)
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"La existencia del producto {respuesta.Detalle} no es suficiente, existencia actual: {respuesta.Existencia}");
            }
            return StatusCode(StatusCodes.Status200OK, true);
        }
    }
}
