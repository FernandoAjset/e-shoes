using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LCDE.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private IRepositorioUsuarios repositorioUsuarios;

        public UsuariosController(UserManager<Usuario> userManager, IRepositorioUsuarios repositorioUsuarios,
            SignInManager<Usuario> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.repositorioUsuarios = repositorioUsuarios;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Registro()
        {
            UsuarioCrearDTO usuario = new UsuarioCrearDTO();
            usuario.Roles = await ObtenerRoles();
            return View(usuario);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(UsuarioCrearDTO modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.Roles = await ObtenerRoles();
                return View(modelo);
            }

            var usuario = new Usuario() { Correo = modelo.Correo, Nombre_usuario = modelo.Nombre_usuario, IdRole = modelo.IdRole };

            var resultado = await userManager.CreateAsync(usuario, password: modelo.Contrasennia);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Usuarios");
            }
            else
            {
                modelo.Roles = await ObtenerRoles();

                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(modelo);
            }
        }



        [AllowAnonymous] //Se agrega para poder ingresar a esta acción sin estar registrado
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }
        //Obtener todos los usuarios
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Usuario> respuesta = await repositorioUsuarios.VerUsuarios();
            return View(respuesta);
        }

        //Obtener Usuarios por ID
        [HttpGet]
        public async Task<IActionResult> Usuarios(int id)
        {
            Usuario usuario = await repositorioUsuarios.BuscarUsuarioId(id);
            return View(usuario);
        }

        //editar Uusario
        //aqui deben de crear una vista para mostrar en el formulario la info del usuario pa editar chtm
        [HttpGet]
        public async Task<ActionResult> Editar(int Id)
        {
            try
            {
                Usuario usuario = await repositorioUsuarios.BuscarUsuarioId(Id);
                if (usuario == null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }
                UsuarioActualizarDTO user = new UsuarioActualizarDTO()
                {
                    Id = usuario.Id,
                    Nombre_usuario = usuario.Nombre_usuario,
                    Correo = usuario.Correo,
                    IdRole = usuario.IdRole,
                    Roles = await ObtenerRoles()
                };
                return View(user);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: ClientesController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(UsuarioActualizarDTO usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    usuario.Roles = await ObtenerRoles();
                    return View(usuario);
                }

                var userExist = await repositorioUsuarios.BuscarUsuarioPorEmail(usuario.Correo);
                if (userExist!=null && userExist.Id != usuario.Id)
                {
                    ModelState.AddModelError(string.Empty, "Correo ya existe.");
                    return View(usuario);
                }

                var usuarioExistente = await userManager.FindByIdAsync(usuario.Id.ToString());
                if (usuarioExistente == null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }

                usuarioExistente.Nombre_usuario = usuario.Nombre_usuario;
                usuarioExistente.Correo = usuario.Correo;
                usuarioExistente.IdRole = usuario.IdRole;

                // Actualizar propiedades de usuario
                bool codigoResult = await repositorioUsuarios.EditarUsuario(usuarioExistente);
                if (codigoResult)
                {
                    if (!string.IsNullOrEmpty(usuario.Contrasennia))
                    {
                        var removePasswordResult = await userManager.RemovePasswordAsync(usuarioExistente);
                        if (removePasswordResult.Succeeded)
                        {
                            var addPasswordResult = await userManager.AddPasswordAsync(usuarioExistente, usuario.Contrasennia);
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        //BORRAR USUARIO
        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            Usuario usuario = await repositorioUsuarios.BuscarUsuarioId(id);

            if (usuario is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarUsuario(int id)
        {
            Usuario usuario = await repositorioUsuarios.BuscarUsuarioId(id);
            if (usuario is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (await repositorioUsuarios.BorrarUsuario(id))
            {
                return Ok(new { success = true });
            }
            return BadRequest();
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerRoles()
        {
            return await repositorioUsuarios.ObtenerRoles();
        }
    }

}
