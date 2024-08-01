﻿using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public class RepositorioProductos
    {
        private readonly string connectionString;
        public RepositorioProductos(IConfiguration configuration)
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
                ,@Operacion
            ", new
            {
                DetalleProducto = producto.Detalle,
                Existencia = producto.Existencia,
                IdCategoria = producto.Id_Categoria,
                IdProveedor = producto.Id_Proveedor,
                IdPromocion = producto.IdPromocion,
                IdProducto = 0,
                PrecioUnidad = producto.PrecioUnidad,
                Operacion = "insert"
            });
            return producto_id;
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
                ,@Operacion
                ", new
                {
                    DetalleProducto = "",
                    Existencia = "",
                    IdCategoria = "",
                    IdProveedor = "",
                    IdPromocion = "",
                    IdProducto = IdProducto,
                    PrecioUnidad = 0,
                    Operacion = "select"
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
                ,@Operacion
                ", new
                {
                    DetalleProducto = producto.Detalle,
                    Existencia = producto.Existencia,
                    IdCategoria = producto.Id_Categoria,
                    IdProveedor = producto.Id_Proveedor,
                    IdPromocion = producto.IdPromocion,
                    IdProducto = producto.Id,
                    PrecioUnidad = producto.PrecioUnidad,
                    Operacion = "update"

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
                ,@Operacion
                    ", new
                {
                    DetalleProducto = "",
                    Existencia = "",
                    IdCategoria = "",
                    IdProveedor = "",
                    IdPromocion = "",
                    IdProducto = IdProducto,
                    PrecioUnidad = 0,
                    Operacion = "delete"
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
                ,@Operacion
                        ", new
            {
                DetalleProducto = "",
                Existencia = "",
                IdCategoria = "",
                IdProveedor = "",
                IdPromocion = "",
                IdProducto = 0,
                PrecioUnidad = 0,
                Operacion = "todo"

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
                ,@Operacion
                ", new
                {
                    DetalleProducto = nombre,
                    Existencia = "",
                    IdCategoria = "",
                    IdProveedor = "",
                    IdPromocion = "",
                    IdProducto = idProducto,
                    PrecioUnidad = 0,
                    Operacion = "selectPorNombre"
                });
                return producto;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}