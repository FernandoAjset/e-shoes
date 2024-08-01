using AspNetCore;
using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.Security.Claims;

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
            UsuarioDTO usuario = new UsuarioDTO();
            usuario.Roles = await ObtenerRoles();
            return View(usuario);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(UsuarioDTO modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.Roles = await ObtenerRoles();
                return View(modelo);
            }

            var usuario = new Usuario() { Correo = modelo.Correo, Nombre_usuario = modelo.Nombre_usuario };

            var resultado = await userManager.CreateAsync(usuario, password: modelo.Contrasennia);

            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
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

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(modelo);
                }

                var resultado = await signInManager.PasswordSignInAsync(modelo.Email,
                    modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);

                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Ventas");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto.");
                    return View(modelo);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
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
                UsuarioDTO user = new UsuarioDTO()
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
        public async Task<ActionResult> Editar(UsuarioDTO usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    usuario.Roles = await ObtenerRoles();
                    return View(usuario);
                }

                var userExist = await repositorioUsuarios.BuscarUsuarioPorEmail(usuario.Correo);
                if (userExist.Id!=usuario.Id)
                {
                    ModelState.AddModelError(string.Empty, "Correo ya existe.");
                    return View(usuario);
                }

                var nuevoUsuario = new Usuario()
                {
                    Id = usuario.Id ?? 0,
                    Nombre_usuario = usuario.Nombre_usuario,
                    Correo = usuario.Correo,
                    IdRole = usuario.IdRole
                };

                bool codigoResult = await repositorioUsuarios.EditarUsuario(nuevoUsuario);
                if (codigoResult)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
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
