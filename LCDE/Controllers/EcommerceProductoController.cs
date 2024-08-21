using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Crmf;

namespace LCDE.Controllers
{
    public class EcommerceProductoController : Controller
    {
        private readonly IrepositorioProducto repositorioProductos;

        public EcommerceProductoController (IrepositorioProducto repositorioProductos)
        {
            this.repositorioProductos = repositorioProductos;
        }

        [HttpGet]
        public async Task<IActionResult> DetalleProducto(int productoId) 
        {
            try
            {
                ProductoListarDTO producto = await repositorioProductos.ObtenerDetalleProducto(productoId);

                if (producto == null) 
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }
                return View(producto);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        


        }
    }
}
