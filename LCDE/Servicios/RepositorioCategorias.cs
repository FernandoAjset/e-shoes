using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public class RepositorioCategorias
    {
        private readonly string connectionString;
        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");

        }

        public async Task<int> CrearCategoria(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            int categoria_id = await connection.QuerySingleAsync<int>(@"
            EXEC SP_CRUD_CATEGORIA_PRODUCTOS @IdCategoria, @Nombre, @Operacion
            ", new

            {
                IdCategoria = 0,
                Nombre = categoria.Nombre,
                Operacion = "insert"
            });
            return categoria_id;
        }
        public async Task<Categoria> ObtenerCategoria(int IdCategoria)
        {
            using var connection = new SqlConnection(connectionString);
            Categoria categoria = await connection.QuerySingleAsync<Categoria>(@"
            EXEC SP_CRUD_CATEGORIA_PRODUCTOS @IdCategoria, @Nombre, @Operacion
            ", new
            {
                IdCategoria = IdCategoria,
                Nombre = "",
                Operacion = "select"
            });
            return categoria;
        }

        public async Task<bool> ModificarCategoria(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            int categoria_id = await connection.QuerySingleAsync<int>(@"
            EXEC SP_CRUD_CATEGORIA_PRODUCTOS @IdCategoria, @Nombre, @Operacion
            ", new
            {
                IdCategoria = categoria.Id,
                Nombre =categoria.Nombre,
                Operacion = "update"
            });
            return categoria_id>0;
        }

        public async Task<bool> BorrarCategoria(int IdCategoria)
        {
            using var connection = new SqlConnection(connectionString);
            int categoria = await connection.QuerySingleAsync<int>(@"
            EXEC SP_CRUD_CATEGORIA_PRODUCTOS @IdCategoria, @Nombre, @Operacion
            ", new
            {
                IdCategoria = IdCategoria,
                Nombre = "",
                Operacion = "delete"
            });
            return categoria > 0;
        }

        public async Task<IEnumerable<Categoria>> ObtenerTodosCategorias()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<Categoria> categoria = await connection.QueryAsync<Categoria>(@"
            EXEC SP_CRUD_CATEGORIA_PRODUCTOS @IdCategoria, @Nombre, @Operacion
            ", new
            {
                IdCategoria = 0,
                Nombre = "",
                Operacion = "todo"

            });
            return categoria;
        }
    }
}
