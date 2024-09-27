using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LCDE.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class ReportesController : Controller
    {
        private readonly RepositorioReportes repositorioReportes;
        private readonly RepositorioCategorias repositorioCategorias;
        private readonly RepositorioVentas repositorioVentas;

        public ReportesController(
             RepositorioReportes repositorioReportes,
            RepositorioCategorias repositorioCategorias,
            RepositorioVentas repositorioVentas
            )
        {
            this.repositorioReportes = repositorioReportes;
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioVentas = repositorioVentas;
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

        [HttpGet]
        public async Task<IActionResult> Facturas()
        {
            IEnumerable<FacturaDTOViewModel> datos = await repositorioReportes.Facturas();
            if (datos is not null)
            {
                return View(datos);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> AnularFactura(int id)
        {
            try
            {
                var factura = await repositorioVentas.ObtenerEncabezadoFacturaPorId(id);
                if (factura == null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }
                await repositorioVentas.AnularFactura(id);
                return RedirectToAction("Facturas");
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
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
