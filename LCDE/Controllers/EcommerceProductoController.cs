using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LCDE.Controllers
{
    public class EcommerceProductoController : Controller
    {
        private readonly RepositorioProductos repositorioProductos;
        private readonly RepositorioCategorias repositorioCategorias;

        private readonly string rootDefaultImg = "https://schoolcampussur.blob.core.windows.net/lcde-productos/652f9c99-a286-43d9-b7a5-28be1e376fbc.jpg";

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

                if (producto.Image_url == null)
                {
                    producto.Image_url = rootDefaultImg;
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
        public async Task<IActionResult> BuscarPorCategorias(int CategoriaId)
        {
            ProductoFiltroDTO obj = new ProductoFiltroDTO();
            obj.Categorias = (await repositorioCategorias.ObtenerTodosCategorias()).ToList();
            obj.CategoriaId = CategoriaId;
            obj.PrecioMin = 0;
            obj.PrecioMax = await repositorioProductos.PrecioMaximo();
            obj.NombreProducto = "";
            var productos = await repositorioProductos.ObtenerProductoFiltrado(obj);


            foreach (var producto in productos)
            {
                if (producto.Image_url == null)
                {
                    producto.Image_url = rootDefaultImg;
                }
            }

            // Obtener listado de proveedores
            var Tallas = await repositorioProductos.ObtenerTallas( obj.CategoriaId );
            var resultadoProveedores = Tallas
                    .Select(x => new SelectListItem(x.ToString(), x.ToString() )).ToList();
            obj.Tallas= resultadoProveedores;
            obj.Talla= Tallas.FirstOrDefault();
            obj.productosListarDTO = productos.OrderBy(x => x.Nombre).ToList();

            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> BuscarPorCategorias(ProductoFiltroDTO productoFiltrar)
        {
            try
            {
                List<ProductoListarDTO> productos = await repositorioProductos.ObtenerProductoFiltrado(productoFiltrar);
                foreach (var producto in productos)
                {
                    if (producto.Image_url == null)
                    {
                        producto.Image_url = rootDefaultImg;
                    }
                }
                return Json(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Hubo un error al filtrar los productos." });
            }
        }
    }
}