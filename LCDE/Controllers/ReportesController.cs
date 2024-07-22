using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LCDE.Controllers
{
    public class ReportesController : Controller
    {
        private readonly RepositorioReportes repositorioReportes;
        private readonly RepositorioCategorias repositorioCategorias;

        public ReportesController(
             RepositorioReportes repositorioReportes,
            RepositorioCategorias repositorioCategorias
            )
        {
            this.repositorioReportes = repositorioReportes;
            this.repositorioCategorias = repositorioCategorias;
        }
        public IActionResult VentasDiariasPorCategoría()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReporteVentasDiariasPorCategoria(string fecha)
        {
            IEnumerable<dynamic> datos = await repositorioReportes.VentasDiariasPorCategoria(fecha);
            if (datos is not null)
            {
                return StatusCode(StatusCodes.Status200OK, datos);
            }
            return RedirectToAction("Error", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> CorteCajaPorTurno()
        {
            IEnumerable<dynamic> datos = await repositorioReportes.CorteCajaPorTurno();
            if (datos is not null)
            {
                return View(datos);
            }
            return RedirectToAction("Error", "Home");
        }

        public async Task<IActionResult> VentasPorCategoria()
        {
            var categorias = await repositorioCategorias.ObtenerTodosCategorias();
            var resultadoCategorias = categorias
                    .Select(x => new SelectListItem(x.Nombre, x.Nombre)).ToList();
            var opcionPorDefectoCategoria = new SelectListItem("-- Seleccione una categoría --", "0", true);
            resultadoCategorias.Insert(0, opcionPorDefectoCategoria);
            return View(new { Categorias = resultadoCategorias });
        }

        [HttpGet]
        public async Task<IActionResult> ReporteVentasPorCategoria(string categoria, string fecha)
        {
            IEnumerable<dynamic> datos = await repositorioReportes.VentasPorCategoria(categoria, fecha);
            if (datos is not null)
            {
                return StatusCode(StatusCodes.Status200OK, datos);
            }
            return RedirectToAction("Error", "Home");
        }

        public async Task<IActionResult> DevolucionesPorCategoria()
        {
            var categorias = await repositorioCategorias.ObtenerTodosCategorias();
            var resultadoCategorias = categorias
                    .Select(x => new SelectListItem(x.Nombre, x.Nombre)).ToList();
            var opcionPorDefectoCategoria = new SelectListItem("-- Seleccione una categoría --", "0", true);
            resultadoCategorias.Insert(0, opcionPorDefectoCategoria);
            return View(new { Categorias = resultadoCategorias });
        }

        [HttpGet]
        public async Task<IActionResult> ReporteDevolucionesPorCategoria(string categoria, string fecha)
        {
            IEnumerable<dynamic> datos = await repositorioReportes.DevolucionesPorCategoria(categoria, fecha);
            if (datos is not null)
            {
                return StatusCode(StatusCodes.Status200OK, datos);
            }
            return RedirectToAction("Error", "Home");
        }
    }
}
