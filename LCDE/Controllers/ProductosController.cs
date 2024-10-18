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
        private readonly string rootDefaultImg = "https://schoolcampussur.blob.core.windows.net/lcde-productos/652f9c99-a286-43d9-b7a5-28be1e376fbc.jpg";

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
            try
            {
                IEnumerable<ProductoListarDTO> productos = await repositorioProductos.ObtenerTodosProductos();

                foreach (var producto in productos)
                {
                    if (producto.Image_url == null)
                    {
                        producto.Image_url = rootDefaultImg;
                    }
                }
                // Agrupar productos por el nombre
                List<ProductosAgrupadosDTO> agrupados = productos.GroupBy(x => x.Nombre)
                                         .Select(grupo => new ProductosAgrupadosDTO
                                         {
                                             Nombre = grupo.Key,
                                             Detalle = grupo.First().Detalle, // Suponiendo que el detalle es el mismo para todos
                                             Image_url = grupo.First().Image_url, // Suponiendo que la URL de la imagen es la misma
                                             NombreCategoria = grupo.First().NombreCategoria, // Suponiendo la misma categoría
                                             NombreProveedor = grupo.First().NombreProveedor, // Suponiendo el mismo proveedor
                                             Productos = grupo.ToList() // Todos los productos en este grupo
                                         }).ToList();
                return View(agrupados);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
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
            if ( (!ModelState.IsValid && producto.Id != 0) || producto.Existencia < producto.Stock_Minimo )
            {
                if (producto.Existencia < producto.Stock_Minimo) {
                    ModelState.AddModelError(nameof(producto.Stock_Minimo), "El stock mínimo no puede ser mayor que la existencia actual.");
                }

                // Obtener listado de categorías
                var categorias = await repositorioCategorias.ObtenerTodosCategorias();
                var resultadoCategorias = categorias
                        .Select(x => new SelectListItem(x.Nombre, x.Id.ToString())).ToList();
                var opcionPorDefectoCategoria = new SelectListItem("-- Seleccione una categoría --", "0", true);
                resultadoCategorias.Insert(0, opcionPorDefectoCategoria);
                producto.Categorias = resultadoCategorias;

                // Obtener listado de proveedores
                var proveedores = await repositorioProveedores.ObtenerTodosProveedores();
                var resultadoProveedores = proveedores
                        .Select(x => new SelectListItem(x.Nombre, x.Id.ToString())).ToList();
                var opcionPorDefectoProveedor = new SelectListItem("-- Seleccione una categoría --", "0", true);
                resultadoProveedores.Insert(0, opcionPorDefectoProveedor);
                producto.Proveedores = resultadoProveedores;
                return View(producto);
            }
            //Enviar la imagen a Azure Storage
            if (producto.Imagen != null) producto.Image_url = await fileRepository.AddFile(producto.Imagen, CarpetaDeImg);

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
                    if (producto.Existencia < producto.Stock_Minimo)
                    {
                        ModelState.AddModelError(nameof(producto.Stock_Minimo), "El stock mínimo no puede ser mayor que la existencia actual.");
                    }
                    List<string> errores = new List<string>();
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        errores.Add(error.ErrorMessage);
                    }
                    ViewData["Errores"] = errores;

                    return View(producto);
                }

                //Enviar la imagen a Azure Storage
                if (producto.Imagen != null && producto.Image_url != this.rootDefaultImg)
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

            if (await repositorioProductos.BorrarProducto(id) && producto.Image_url != this.rootDefaultImg)
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

        [HttpGet]
        public async Task<IActionResult> CrearTalla(int Id)
        {
            ProductoCreacionDTO producto = await repositorioProductos.ObtenerProducto(Id);
            if (producto == null)
            {
                return NotFound();
            }

            producto.Existencia = 0;
            producto.Stock_Minimo = 0;
            producto.talla = 0;
            producto.PrecioUnidad = 0;
            producto.PrecioUnidadString = "0";
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearTalla(ProductoCreacionDTO Product)
        {
            try
            {
                ProductoCreacionDTO producto = new ProductoCreacionDTO();

                producto.talla = Product.talla;
                producto.Id_Categoria = Product.Id_Categoria;
                producto.Id_Proveedor = Product.Id_Proveedor;
                producto.Detalle = Product.Detalle;
                producto.Existencia = Product.Existencia;
                producto.Stock_Minimo = Product.Stock_Minimo;
                producto.PrecioUnidad = Product.PrecioUnidad;
                producto.PrecioUnidadString = Product.PrecioUnidadString;
                producto.Image_url = Product.Image_url;
                producto.IdPrecio = Product.IdPrecio;
                producto.IdPromocion = Product.IdPromocion;
                producto.Proveedores = Product.Proveedores;
                producto.Categorias = Product.Categorias;
                producto.Image_url = Product.Image_url;
                producto.Nombre = Product.Nombre;
                producto.Imagen = Product.Imagen;


                CultureInfo culture = CultureInfo.InvariantCulture;
                decimal cantidad;
                string valorIngresado = producto.PrecioUnidadString; // Valor ingresado en el campo de texto

                if (decimal.TryParse(valorIngresado, NumberStyles.Number, culture, out cantidad))
                {
                    producto.PrecioUnidad = cantidad;
                }
                // Validación de las validaciones del modelo.
                if ((!ModelState.IsValid && producto.Id != 0) || producto.Existencia < producto.Stock_Minimo)
                {
                    if (producto.Existencia < producto.Stock_Minimo)
                    {
                        ModelState.AddModelError(nameof(producto.Stock_Minimo), "El stock mínimo no puede ser mayor que la existencia actual.");
                    }

                    // Obtener listado de categorías
                    var categorias = await repositorioCategorias.ObtenerTodosCategorias();
                    var resultadoCategorias = categorias
                            .Select(x => new SelectListItem(x.Nombre, x.Id.ToString())).ToList();
                    var opcionPorDefectoCategoria = new SelectListItem("-- Seleccione una categoría --", "0", true);
                    resultadoCategorias.Insert(0, opcionPorDefectoCategoria);
                    producto.Categorias = resultadoCategorias;

                    // Obtener listado de proveedores
                    var proveedores = await repositorioProveedores.ObtenerTodosProveedores();
                    var resultadoProveedores = proveedores
                            .Select(x => new SelectListItem(x.Nombre, x.Id.ToString())).ToList();
                    var opcionPorDefectoProveedor = new SelectListItem("-- Seleccione una categoría --", "0", true);
                    resultadoProveedores.Insert(0, opcionPorDefectoProveedor);
                    producto.Proveedores = resultadoProveedores;
                    return View(producto);
                }
                //Enviar la imagen a Azure Storage
                if (producto.Imagen != null) producto.Image_url = await fileRepository.AddFile(producto.Imagen, CarpetaDeImg);

                else producto.Image_url = rootDefaultImg;

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
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditarGroup(int Id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarGroup(ProductoCreacionDTO producto)
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
                    if (producto.Existencia < producto.Stock_Minimo)
                    {
                        ModelState.AddModelError(nameof(producto.Stock_Minimo), "El stock mínimo no puede ser mayor que la existencia actual.");
                    }
                    List<string> errores = new List<string>();
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        errores.Add(error.ErrorMessage);
                    }
                    ViewData["Errores"] = errores;

                    return View(producto);
                }

                //Enviar la imagen a Azure Storage
                if (producto.Imagen != null && producto.Image_url != this.rootDefaultImg)
                {
                    await fileRepository.DeleteFile(producto.Image_url, CarpetaDeImg);
                    producto.Image_url = await fileRepository.AddFile(producto.Imagen, CarpetaDeImg);
                }
                int affectedRows = await repositorioProductos.ActualizarGrupoProductos(producto);
                if (affectedRows > 0)
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
    }
}
