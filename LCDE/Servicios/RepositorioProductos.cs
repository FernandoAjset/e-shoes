using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public class RepositorioProductos
    {
        private readonly string connectionString;
        public RepositorioProductos(IConfiguration configuration)//chingadamadre otro sin interfaz
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");

        }
        public async Task<int> CrearProducto(ProductoCreacionDTO producto)
        {
            using var connection = new SqlConnection(connectionString);
            int producto_id = await connection.QuerySingleAsync<int>(@"
                 EXEC SP_CRUD_PRODUCTOS 
                 @DetalleProducto, @Existencia, @IdCategoria
                ,@IdProveedor,@IdPromocion, @IdProducto,@PrecioUnidad
                ,@Nombre, @ImageUrl, @talla, @Operacion, @minimum", new
            {
                DetalleProducto = producto.Detalle,
                Existencia = producto.Existencia,
                IdCategoria = producto.Id_Categoria,
                IdProveedor = producto.Id_Proveedor,
                IdPromocion = producto.IdPromocion,
                IdProducto = 0,
                PrecioUnidad = producto.PrecioUnidad,
                Nombre = producto.Nombre,
                talla = producto.talla,
                ImageUrl = producto.Image_url,
                Operacion = "insert",
                minimum = producto.Stock_Minimo
            });
            return producto_id;
        }

        public async Task<IEnumerable<CarritoItemDTO>> ObtenerDetallesProductos(IEnumerable<int> idsProductos)
        {
            using var connection = new SqlConnection(connectionString);
            var productos = await connection.QueryAsync<CarritoItemDTO>(@"
                                EXEC sp_ObtenerListadoDetallesProductos @IdsProductos
                            ", new
            {
                IdsProductos = string.Join(",", idsProductos)
            });
            return productos;
        }

        public async Task<ProductoCreacionDTO> ObtenerProducto(int IdProducto)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                ProductoCreacionDTO producto = await connection.QuerySingleAsync<ProductoCreacionDTO>(@"
                 EXEC SP_CRUD_PRODUCTOS 
                 @DetalleProducto, @Existencia, @IdCategoria
                ,@IdProveedor,@IdPromocion, @IdProducto,@PrecioUnidad
                ,@Nombre, @ImageUrl, @talla, @Operacion, @minimum", new
                {
                    DetalleProducto = "",
                    Existencia = "",
                    IdCategoria = "",
                    IdProveedor = "",
                    IdPromocion = "",
                    IdProducto = IdProducto,
                    PrecioUnidad = 0,
                    Nombre = "",
                    ImageUrl = "",
                    talla = 0.0,
                    Operacion = "select",
                    minimum = ""
                });
                return producto;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> ModificarProducto(ProductoCreacionDTO producto)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync(@"
                 EXEC SP_CRUD_PRODUCTOS 
                 @DetalleProducto, @Existencia, @IdCategoria
                ,@IdProveedor,@IdPromocion, @IdProducto,@PrecioUnidad
                ,@Nombre, @ImageUrl, @talla, @Operacion, @minimum", new
                {
                    DetalleProducto = producto.Detalle,
                    Existencia = producto.Existencia,
                    IdCategoria = producto.Id_Categoria,
                    IdProveedor = producto.Id_Proveedor,
                    IdPromocion = producto.IdPromocion,
                    IdProducto = producto.Id,
                    PrecioUnidad = producto.PrecioUnidad,
                    Nombre = producto.Nombre,
                    ImageUrl = producto.Image_url,
                    talla = producto.talla,
                    Operacion = "update",
                    minimum = producto.Stock_Minimo

                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> BorrarProducto(int IdProducto)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync(@"
                EXEC SP_CRUD_PRODUCTOS 
                 @DetalleProducto, @Existencia, @IdCategoria
                ,@IdProveedor,@IdPromocion, @IdProducto,@PrecioUnidad
                ,@Nombre, @ImageUrl, @talla, @Operacion, @minimum", new
                {
                    DetalleProducto = "",
                    Existencia = "",
                    IdCategoria = "",
                    IdProveedor = "",
                    IdPromocion = "",
                    IdProducto = IdProducto,
                    PrecioUnidad = 0,
                    Nombre = "",
                    ImageUrl = "",
                    talla = 0.0,
                    Operacion = "delete",
                    minimum = ""
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<ProductoListarDTO>> ObtenerTodosProductos()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<ProductoListarDTO> producto = await connection.QueryAsync<ProductoListarDTO>(@"
                EXEC SP_CRUD_PRODUCTOS 
                 @DetalleProducto, @Existencia, @IdCategoria
                ,@IdProveedor,@IdPromocion, @IdProducto,@PrecioUnidad
                ,@Nombre, @ImageUrl, @talla, @Operacion, @minimum", new
            {
                DetalleProducto = "",
                Existencia = "",
                IdCategoria = "",
                IdProveedor = "",
                IdPromocion = "",
                IdProducto = 0,
                PrecioUnidad = 0,
                Nombre = "",
                ImageUrl = "",
                talla = 0.0,
                Operacion = "todo",
                minimum = ""
            });
            return producto;
        }

        public async Task<Producto> ObtenerProductoPorNombre(int idProducto, string nombre)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                Producto producto = await connection.QueryFirstOrDefaultAsync<Producto>(@"
                EXEC SP_CRUD_PRODUCTOS 
                 @DetalleProducto, @Existencia, @IdCategoria
                ,@IdProveedor,@IdPromocion, @IdProducto,@PrecioUnidad 
                , @talla,@Operacion, @minimum
                ", new
                {
                    DetalleProducto = nombre,
                    Existencia = "",
                    IdCategoria = "",
                    IdProveedor = "",
                    IdPromocion = "",
                    IdProducto = idProducto,
                    PrecioUnidad = 0,
                    talla = 0.0,
                    Operacion = "selectPorNombre",
                    minimum = ""
                });
                return producto;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<decimal> PrecioMaximo()
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                var precio = await connection.QueryFirstOrDefaultAsync<dynamic>(@"
                EXEC sp_ObtenerPrecioMaximo");
                return precio != null ? precio.precio_unidad : 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<ProductoListarDTO> ObtenerDetalleProducto(int idProducto)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                ProductoListarDTO producto = await connection.QueryFirstOrDefaultAsync<ProductoListarDTO>(@"
                 EXEC sp_ObtenerDetalleProducto 
                  @IdProducto
                         ", new { IdProducto = idProducto });
                return producto;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<List<ProductoListarDTO>> ObtenerProductoFiltrado(ProductoFiltroDTO productoFiltrar)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                List<ProductoListarDTO> ListaFiltrada = (await connection.QueryAsync<ProductoListarDTO>(@"
                EXEC sp_FiltrarProductos @categoriaId, @nombreProducto, @precioMin, @precioMax, @talla
                ", new
                {
                    categoriaId = productoFiltrar.CategoriaId,
                    nombreProducto = productoFiltrar.NombreProducto,
                    precioMin = productoFiltrar.PrecioMin,
                    precioMax = productoFiltrar.PrecioMax,
                    talla = productoFiltrar.Talla
                })).ToList();
                return ListaFiltrada;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> ActualizarGrupoProductos(ProductoCreacionDTO producto)
        {
            using var connection = new SqlConnection(connectionString);

            // Ejecutar el procedimiento almacenado para actualizar los campos
            int affectedRows = await connection.ExecuteScalarAsync<int>(@"
                EXEC SP_CRUD_PRODUCTOS 
                @DetalleProducto, @Existencia, @IdCategoria,
                @IdProveedor, @IdPromocion, @IdProducto,
                @PrecioUnidad, @Nombre, @ImageUrl, @talla, 
                @Operacion, @minimum", new
                    {
                        DetalleProducto = producto.Detalle,
                        Existencia = producto.Existencia, // Aunque no se modifica en esta operación, lo envías.
                        IdCategoria = producto.Id_Categoria,
                        IdProveedor = producto.Id_Proveedor,
                        IdPromocion = producto.IdPromocion, // Este puede no ser requerido en este contexto.
                        IdProducto = producto.Id, // Se utilizará este para buscar el producto base.
                        PrecioUnidad = producto.PrecioUnidad, // Igual que existencia, se envía pero no se usa aquí.
                        Nombre = producto.Nombre,
                        ImageUrl = producto.Image_url,
                        talla = producto.talla, // Aunque no se actualiza, se sigue enviando.
                        Operacion = "updatePorNombre",
                        minimum = producto.Stock_Minimo // Este también es enviado por consistencia.
                    });

            return affectedRows; // Devuelve el número de filas afectadas.
        }

        public async Task<List<float>> ObtenerTallas(int CategoriaId) {
            using var connection = new SqlConnection(connectionString);
            List<float> tallas = (await connection.QueryAsync<float>(@"
                select talla from productos where id_categoria = @CategoriaId group by talla;", new { CategoriaId })).ToList();
        
            return tallas;
        }
    }
}
