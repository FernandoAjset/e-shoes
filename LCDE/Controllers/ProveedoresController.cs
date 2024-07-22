using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace LCDE.Controllers
{
    /// <summary>
    /// Controlador con acciones relacionadas a la entidad de Cliente.
    /// </summary>
    public class ProveedoresController : Controller
    {
        private readonly RepositorioProveedores repositorioProveedores;

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="repositorioProveedores">Inyección del servicio de repositorio de clientes, que realiza el acceso a la base de datos.</param>
        public ProveedoresController(RepositorioProveedores repositorioProveedores)
        {
            this.repositorioProveedores = repositorioProveedores;
        }
        /// <summary>
        /// Enpoint para devolver la vista de listado de clientes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var clientes = await repositorioProveedores.ObtenerTodosProveedores();
            return View(clientes);
        }

        /// <summary>
        /// Endpoint para devolver la vista de creación de un nuevo cliente.
        /// </summary>
        /// <returns>La vista del formulario de creación.</returns>
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        /// <summary>
        /// Endpoint al realizar el submit del formulario de creación del registro.
        /// </summary>
        /// <param name="cliente">El objeto tipo Cliente que contiene los datos del registro.</param>
        /// <returns>Una acción al index o a la vista de error.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Proveedor proveedor)
        {
            // Validación de las validaciones del modelo.
            if (!ModelState.IsValid && proveedor.Id != 0)
            {
                return View(proveedor);
            }
            // Enviar los datos al repositorio para grabar en base de datos, si se crea el registro se obtiene el nuevo Id.
            if (await repositorioProveedores.CrearProveedor(proveedor) > 0)
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
                Proveedor proveedor = await repositorioProveedores.ObtenerProveedor(Id);
                if (proveedor == null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }
                return View(proveedor);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: ClientesController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(Proveedor proveedor)
        {
            try
            {
                bool codigoResult = await repositorioProveedores.ModificarProveedor(proveedor);
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
            var proveedor = await repositorioProveedores.ObtenerProveedor(id);

            if (proveedor is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(proveedor);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarProveedor(int id)
        {
            var proveedor = await repositorioProveedores.ObtenerProveedor(id);
            if (proveedor is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (await repositorioProveedores.BorrarProveedor(id))
            {
                return Ok(new { success = true });
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> ProveedorExiste(string NIT, int Id)
        {
            Proveedor respuesta = await repositorioProveedores.ObtenerProveedorPorNit(NIT, Id);
            if (respuesta is not null)
            {
                return Json($"Ya existe un proveedor con NIT {NIT}");
            }
            return Json(true);
        }
    }
}
