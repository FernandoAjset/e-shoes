using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace LCDE.Controllers
{
    /// <summary>
    /// Controlador con acciones relacionadas a la entidad de Cliente.
    /// </summary>
    public class ClientesController : Controller
    {
        private readonly RepositotioClientes repositotioClientes;

        /// <summary>
        /// Constructor de clase.
        /// </summary>
        /// <param name="repositotioClientes">Inyección del servicio de repositorio de clientes, que realiza el acceso a la base de datos.</param>
        public ClientesController(RepositotioClientes repositotioClientes)
        {
            this.repositotioClientes = repositotioClientes;
        }
        /// <summary>
        /// Enpoint para devolver la vista de listado de clientes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var clientes = await repositotioClientes.ObtenerTodosClientes();
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
        public async Task<IActionResult> Crear(Cliente cliente)
        {
            // Validación de las validaciones del modelo.
            if (!ModelState.IsValid && cliente.Id != 0)
            {
                return View(cliente);
            }
            // Enviar los datos al repositorio para grabar en base de datos, si se crea el registro se obtiene el nuevo Id.
            if (await repositotioClientes.CrearCliente(cliente) > 0)
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
                Cliente cliente = await repositotioClientes.ObtenerCliente(Id);
                if (cliente == null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }
        //quien chingados pone un post en un Put
        // POST: ClientesController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(Cliente cliente)
        {
            try
            {
                bool codigoResult = await repositotioClientes.ModificarCliente(cliente);
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
            var cliente = await repositotioClientes.ObtenerCliente(id);

            if (cliente is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCliente(int id)
        {
            var cliente = await repositotioClientes.ObtenerCliente(id);
            if (cliente is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (await repositotioClientes.BorrarCliente(id))
            {
                return Ok(new { success = true });
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> ClienteExiste(string NIT, int Id)
        {
            Cliente respuesta = await repositotioClientes.ObtenerClientePorNit(NIT, Id);
            if (respuesta is not null)
            {
                return Json($"Ya existe un cliente con NIT {NIT}");
            }
            return Json(true);
        }
    }
}
