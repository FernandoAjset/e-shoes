using Dapper;
using LCDE.Models;
using Microsoft.Data.SqlClient;

namespace LCDE.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(Usuario usuario);
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
    }
}
