﻿using Dapper;
using LCDE.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        Task<IEnumerable<SelectListItem>> ObtenerRoles();
    }

    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;
        public RepositorioUsuarios(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConnectionLCDE");
        }

        public async Task<int> CrearUsuario(Usuario usuario)///////////////////////////////////////
        {//EXEC SP_CREAR_USUARIO @NombreUsuario, @Contrasennia, @Correo
            using var connection = new SqlConnection(connectionString);
            var usuarioId = await connection.QuerySingleAsync<int>(@"
                        EXEC SP_CREAR_USUARIOS @nombre_usuario, @contrasennia, @correo, @id_role
                        ", new
            {
                nombre_usuario = usuario.Nombre_usuario,
                Contrasennia = usuario.Contrasennia,
                correo = usuario.Correo,
                id_role = usuario.Id_Role
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
        public async Task<List<Usuario>> VerUsuarios()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<Usuario> user = await connection.QueryAsync<Usuario>(@"
                        EXEC SP_OBTENER_TODOS_USUARIOS 
                        ");
            return user.ToList();
        }

        //AQUI FALTA SOLO INGRESAR EL SP DE OBTENER USUARIO POR ID
        public async Task<Usuario> BuscarUsuarioId(int id)
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<Usuario> user = await connection.QueryAsync<Usuario>(@"
                        EXEC SP_OBTENER_OBTENER_POR_ID @idusuario 
                        ", new
            {
                idusuario = id
            });
            return user.FirstOrDefault();
        }
        //SP PARA EDITAR USUARIO
        public async Task<bool> EditarUsuario(Usuario usuario)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);//Yaaaaaaaaa
                await connection.ExecuteAsync(@"
                       EXEC SP_EDITAR_USUARIO @idusuario, @nombre_usuario, @contrasennia, @correo, @Id_role
                        ", new
                {
                    idusuario = usuario.Id,
                    nombre_usuario = usuario.Nombre_usuario,
                    contrasennia = usuario.Contrasennia??"",
                    correo = usuario.Correo,
                    Id_role = usuario.Id_Role
                });
                return true;
            }
            catch (Exception ex)
            {
                throw;
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
                        EXEC SP_ELIMINAR_USUARIOS @idusuario
                        ", new { idusuario = IdUser });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<SelectListItem>> ObtenerRoles()
        {
            using var connection = new SqlConnection(connectionString);
            IEnumerable<SelectListItem> user = await connection.QueryAsync<SelectListItem>(@"
                        select Nombre AS Text, Id AS Value from rol 
                        ");
            return user.ToList();
        }
    }
}