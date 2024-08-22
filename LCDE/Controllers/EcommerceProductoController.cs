using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LCDE.Controllers
{
    public class EcommerceProductoController : Controller
    {
        private readonly RepositorioProductos repositorioProductos;
        private readonly RepositorioCategorias repositorioCategorias;


        public EcommerceProductoController(

            RepositorioProductos repositorioProductos, RepositorioCategorias repositorioCategorias)
        {
            this.repositorioProductos = repositorioProductos;
            this.repositorioCategorias = repositorioCategorias;
        }
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
        public async Task<IActionResult> BuscarPorCategorias()
        {
            ProductoFiltroDTO obj = new ProductoFiltroDTO();
            obj.Categorias = (await repositorioCategorias.ObtenerTodosCategorias()).ToList();
            obj.PrecioMin = 0;
            obj.PrecioMax = await repositorioProductos.PrecioMaximo();
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> BuscarPorCategorias(ProductoFiltroDTO productoFiltrar)
        { 
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(productoFiltrar);
                }
               
                List<ProductoListarDTO> filtro = await repositorioProductos.ObtenerProductoFiltrado(productoFiltrar);

                // Validación de que se encontró alguna categoría
                if (filtro == null || !filtro.Any())
                {
                    // Si no se encontró la categoría, redirigir a la acción "NoEncontrado"
                    return RedirectToAction("NoEncontrado", "Home");
                }
                return View(filtro);//Aqui tengo una duda, no sé cual es el tipo de dato que lleva la vista que se va a retornar
            }
            catch (Exception ex) { return RedirectToAction("Error", "Home"); }
        }
    }

}