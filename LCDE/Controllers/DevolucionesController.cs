using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace LCDE.Controllers
{
    public class DevolucionesController : Controller
    {

        private readonly RepositorioProductos repositorioProductos;
        private readonly RepositorioDevoluciones repositorioDevoluciones;

        public DevolucionesController(
                    RepositorioProductos repositorioProductos,
                    RepositorioDevoluciones repositorioDevoluciones
                    )
        {
            this.repositorioProductos = repositorioProductos;
            this.repositorioDevoluciones = repositorioDevoluciones;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var devoluciones = await repositorioDevoluciones.ObtenerTodosDevoluciones();
            return View(devoluciones);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {

            var modelo = new DevolucionCrear();
            // Obtener listado de categorías
            var productos = await repositorioProductos.ObtenerTodosProductos();
            var resultadoProductos = productos
                    .Select(x => new SelectListItem(x.Detalle, x.Id.ToString())).ToList();
            var opcionPorDefectoProducto = new SelectListItem("-- Seleccione un producto --", "0", true);
            resultadoProductos.Insert(0, opcionPorDefectoProducto);
            modelo.Productos = resultadoProductos;

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(DevolucionCrear devolucion)
        {
            // Validación de las validaciones del modelo.
            if (!ModelState.IsValid)
            {
                return View(devolucion);
            }
            devolucion.Fecha = DateTime.Now;
            // Enviar los datos al repositorio para grabar en base de datos, si se crea el registro se obtiene el nuevo Id.
            if (await repositorioDevoluciones.CrearDevolucion(devolucion) > 0)
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

        [HttpPost]
        public async Task<IActionResult> BorrarDevolucion(int id)
        {
            var devolucion = await repositorioDevoluciones.ObtenerDevolucion(id);
            if (devolucion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (await repositorioDevoluciones.BorrarDevolucion(id))
            {
                return Ok(new { success = true });
            }
            return BadRequest();
        }
    }
}
