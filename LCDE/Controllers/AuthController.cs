using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LCDE.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly IRepositorioUsuarios repositorioUsuarios;

        public AuthController(UserManager<Usuario> userManager, IRepositorioUsuarios repositorioUsuarios,
            SignInManager<Usuario> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.repositorioUsuarios = repositorioUsuarios;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Unauthorized()
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
                    var usuario = await repositorioUsuarios.BuscarUsuarioPorEmail(modelo.Email);
                    var claims = new List<Claim>
                        {
                            new(ClaimTypes.Name, usuario.Nombre_usuario), // Agregar el claim del nombre
                            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()), // Agregar el claim del Id de usuario
                            new(ClaimTypes.Role, usuario.Id_Role.ToString())
                        };

                    var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Crear un nuevo principal con los claims adicionales
                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));


                    //await signInManager.SignInAsync(usuario, modelo.Recuerdame);
                    return RedirectToAction("Index", "Ventas");
                }

                ModelState.AddModelError(string.Empty, "Credenciales no válidas");
                return View(modelo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}