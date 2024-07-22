using LCDE.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LCDE.Controllers
{
    public class PromosController : Controller
    {
        private readonly RepositorioPromociones repositorioPromociones;

        public PromosController(RepositorioPromociones repositorioPromociones)
        {
            this.repositorioPromociones = repositorioPromociones;
        }

        [AllowAnonymous]
        public async Task<IActionResult> PromoFactura(int idFactura)
        {
            ViewBag.IdFactura = idFactura;
            var datos = await repositorioPromociones.PromocionesFactura(idFactura);
            return View(datos);
        }
    }
}
