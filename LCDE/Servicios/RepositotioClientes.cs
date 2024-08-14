using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LCDE.Servicios
{
    public interface IRepositorioCliente
    {
        Task<bool> BorrarCliente(int IdCliente);
        Task<int> CrearCliente(Cliente cliente);
        Task<bool> ModificarCliente(Cliente cliente);
        Task<Cliente> ObtenerCliente(int IdCliente);
        Task<Cliente> ObtenerClientePorIdUsuario(int IdUsuario);
        Task<Cliente> ObtenerClientePorNit(string NIT, int Id);
        Task<IEnumerable<Cliente>> ObtenerTodosClientes();
    }
    public class RepositotioClientes : IRepositorioCliente// clase
    {
        private readonly string connectionString;
        public RepositotioClientes(IConfiguration configuration) //cosntructor que se llama igual que la clase
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");
        }

        public async Task<int> CrearCliente(Cliente cliente)
        {
            int cliente_id = 0;
            using var connection = new SqlConnection(connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@NombreCliente", cliente.Nombre);
            parameters.Add("@DireccionCliente", cliente.Direccion);
            parameters.Add("@TelefonoCliente", cliente.Telefono);
            parameters.Add("@CorreoCliente", cliente.Correo);
            parameters.Add("@NIT", cliente.NIT);
            parameters.Add("@IdCliente", 0);
            parameters.Add("@id_usuario", cliente.Id_usuario);
            parameters.Add("@Operacion", "insert");
            cliente_id = await connection.QuerySingleAsync<int>("SP_CRUD_CLIENTES", parameters, commandType: CommandType.StoredProcedure);
            return cliente_id;
        }

        public async Task<Cliente> ObtenerClientePorIdUsuario(int IdUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            Cliente cliente = await connection.QuerySingleOrDefaultAsync<Cliente>(@"
                        EXEC SP_CRUD_CLIENTES 
                        @NombreCliente, 
                        @DireccionCliente, @TelefonoCliente, 
                        @CorreoCliente, @NIT,
                        @IdCliente, @id_usuario,@Operacion
                        ", new
            {
                NombreCliente = "",
                DireccionCliente = "",
                TelefonoCliente = 0,
                CorreoCliente = "",
                NIT = "",
                IdCliente = 0,
                id_usuario = IdUsuario,
                Operacion = "selectPorUsuarioId"
            });
            return cliente;
        }

        public async Task<Cliente> ObtenerCliente(int IdCliente)
        {
            using var connection = new SqlConnection(connectionString);
            Cliente cliente = await connection.QuerySingleOrDefaultAsync<Cliente>(@"
                        EXEC SP_CRUD_CLIENTES 
                        @NombreCliente, 
                        @DireccionCliente, @TelefonoCliente, 
                        @CorreoCliente, @NIT,
                        @IdCliente, @id_usuario,@Operacion
                        ", new
            {
                NombreCliente = "",
                DireccionCliente = "",
                TelefonoCliente = 0,
                CorreoCliente = "",
                NIT = "",
                IdCliente = IdCliente,
                id_usuario = 0,
                Operacion = "select"
            });
            return cliente;
        }

        public async Task<Cliente> ObtenerClientePorNit(string NIT, int Id)
        {
            Cliente cliente = new();
            try
            {
                using var connection = new SqlConnection(connectionString);
                cliente = await connection.QueryFirstOrDefaultAsync<Cliente>(@"
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @NIT, @IdCliente,@Operacion
                        ", new
                {
                    NombreCliente = "",
                    DireccionCliente = "",
                    TelefonoCliente = 0,
                    CorreoCliente = "",
                    NIT = NIT,
                    IdCliente = Id,
                    Operacion = "selectPorNit"
                });
                return cliente;
            }
            catch (Exception ex)
            {
                return cliente;
            }
        }

        public async Task<bool> ModificarCliente(Cliente cliente)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync(@"
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @NIT, @IdCliente, @Operacion
                        ", new
                {
                    NombreCliente = cliente.Nombre,
                    DireccionCliente = cliente.Direccion,
                    TelefonoCliente = cliente.Telefono,
                    CorreoCliente = cliente.Correo,
                    NIT = cliente.NIT,
                    IdCliente = cliente.Id,
                    Operacion = "update"
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> BorrarCliente(int IdCliente)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync(@"
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @NIT, @IdCliente, @Operacion
                        ", new
                {
                    NombreCliente = "",
                    DireccionCliente = "",
                    TelefonoCliente = "",
                    CorreoCliente = "",
                    NIT = "",
                    IdCliente,
                    Operacion = "delete"
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Cliente>> ObtenerTodosClientes()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<Cliente> clientes = await connection.QueryAsync<Cliente>(@"
                        EXEC SP_CRUD_CLIENTES @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @NIT, @IdCliente, @Operacion
                        ", new
            {
                NombreCliente = "",
                DireccionCliente = "",
                TelefonoCliente = 0,
                CorreoCliente = "",
                NIT = "",
                IdCliente = 0,
                Operacion = "todo"
            });
            return clientes;
        }

    }
}