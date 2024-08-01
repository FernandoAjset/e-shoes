using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(Usuario usuario);
        Task<List<Usuario>> VerUsuarios();
        Task<Usuario> BuscarUsuarioId(int id);
        Task<bool> EditarUsuario(Usuario usuario);
        Task<bool> BorrarUsuario(int id);
    }

    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;
        public RepositorioUsuarios(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            using var connection = new SqlConnection(connectionString);
            var usuarioId = await connection.QuerySingleAsync<int>(@"
                        EXEC SP_CREAR_USUARIO @NombreUsuario, @Contrasennia, @Correo
                        ", new
            {
                NombreUsuario = usuario.Nombre_usuario,
                Contrasennia = usuario.Contrasennia,
                Correo = usuario.Correo
            });
            return usuarioId;
        }

        public async Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(connectionString);
            var usuario = await connection.QuerySingleOrDefaultAsync<Usuario>(
                "EXEC SP_OBTENER_USUARIO @NombreUsuario,@Contrasennia,@Correo",
                new { NombreUsuario = "", Contrasennia = "", Correo = emailNormalizado });
            return usuario;
        }

        //AQUI FALTA EL SP PARA OBTENER TODOS LOS USUARIOS
        public async Task <List<Usuario>> VerUsuarios()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<Usuario> user = await connection.QueryAsync<Usuario>(@"
                        select * from usuarios 
                        ");
            return user.ToList();
        }

        //AQUI FALTA SOLO INGRESAR EL SP DE OBTENER USUARIO POR ID
        public async Task<Usuario> BuscarUsuarioId(int id)
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<Usuario> user = await connection.QueryAsync<Usuario>(@"
                        select * from usuarios  @id 
                        ", new { id,
                                Operacion = "select"});
            return user.FirstOrDefault();
        }
        //SP PARA EDITAR USUARIO
        public async Task<bool> EditarUsuario(Usuario usuario)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync(@"
                        EXEC  @id, @NombreUsuario, @correo, @Operacion
                        ", new
                {
                    id = usuario.Id,
                    NombreUsuario = usuario.Nombre_usuario,
                    correo = usuario.Correo,
                    Operacion = "update"
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //sp para eliminar USUARIO
        //Borrar usuarios, asi como ella te borro de su corazón chtmouser!
        public async Task<bool> BorrarUsuario(int IdUser)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.ExecuteAsync(@"
                        EXEC  @NombreCliente, @DireccionCliente, @TelefonoCliente, @CorreoCliente, @NIT, @IdCliente, @Operacion
                        ", new
                {
                    NombreCliente = "",
                    DireccionCliente = "",
                    TelefonoCliente = "",
                    CorreoCliente = "",
                    NIT = "",
                    IdUser,
                    Operacion = "delete"
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
