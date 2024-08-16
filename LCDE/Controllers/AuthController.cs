using LCDE.Models;
using LCDE.Models.Enums;
using LCDE.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace LCDE.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly IRepositorioUsuarios repositorioUsuarios;
        private readonly IRepositorioCliente repositorioCliente;
        private readonly IRepositorioToken repositorioToken;
        private readonly IEmailService emailService;

        public AuthController(
            IConfiguration configuration,
            UserManager<Usuario> userManager,
            IRepositorioUsuarios repositorioUsuarios,
            IRepositorioCliente repositorioCliente,
            IRepositorioToken repositorioToken,
            IEmailService emailService,
            SignInManager<Usuario> signInManager)
        {
            this.repositorioUsuarios = repositorioUsuarios;
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.repositorioCliente = repositorioCliente;
            this.repositorioToken = repositorioToken;
            this.emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmarRegistro([FromQuery] string token)
        {
            try
            {
                var tokenbd = repositorioToken.ObtenerToken(token);
                if (tokenbd == null)
                {
                    ViewData["Error"] = "Token no valido.";
                    return View();
                }
                if (tokenbd.Activo == false)
                {
                    ViewData["Error"] = "Token no valido.";
                    return View();
                }
                int resultadoFechas = DateTime.Compare(tokenbd.Fecha_Vencimiento, DateTime.Now);
                if (resultadoFechas < 0)
                {
                    ViewData["Error"] = "Token no valido.";
                    return View();
                }
                tokenbd.Activo = false;
                if (!repositorioToken.ActualizarToken(tokenbd))
                {
                    return RedirectToAction("Error", "Home");
                }

                var usuarioExistente = await userManager.FindByIdAsync(tokenbd.Id_Usuario.ToString());
                if (usuarioExistente == null)
                {
                    return RedirectToAction("Error", "Home");
                }

                usuarioExistente.Confirmado = true;
                bool codigoResult = await repositorioUsuarios.EditarUsuario(usuarioExistente);
                if (codigoResult)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registro(CrearUsarioCliente modelo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(modelo);
                }

                var usuarioExistente = await userManager.FindByEmailAsync(modelo.informacionUsuario.Correo);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("ErrorValidacion", "El correo ya está en uso.");
                    return View(modelo);
                }

                var usuario = new Usuario()
                {
                    Correo = modelo.informacionUsuario.Correo,
                    Nombre_usuario = modelo.informacionUsuario.Nombre_usuario,
                    Id_Role = (int)Rol.Cliente
                };
                var resultadoUsuario = await userManager.CreateAsync(usuario, password: modelo.informacionUsuario.Contrasennia);

                var cliente = new Cliente()
                {
                    Correo = modelo.informacionUsuario.Correo,
                    Nombre = modelo.informacionCliente.Nombre,
                    Direccion = modelo.informacionCliente.Direccion,
                    Telefono = modelo.informacionCliente.Telefono,
                    Id_usuario = usuario.Id
                    //Id_usuario = 0
                };
                var resultadoCliente = await repositorioCliente.CrearCliente(cliente);

                var token = repositorioToken.CrearTokenRegistroUsuario(usuario);
                try
                {
                    var getTemplate = LeerTemplateService.GetTemplateToStringByName($"confirmar-registro.html");

                    var url = $"{configuration["AppUrl"]}/auth/ConfirmarRegistro?token={token}";

                    var emailBody = getTemplate.Replace("{url}", url);

                    emailBody = emailBody.Replace("{usuario}", cliente.Nombre);


                    await emailService.SendEmailAsync(usuario.Correo, "Confirmar registro", emailBody);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home");
                }
                if (resultadoUsuario.Succeeded && resultadoCliente != 0)

                {

                    return RedirectToAction("Login");
                }
                else
                {

                    ModelState.AddModelError(string.Empty, "Ocurrió un error, intente más tarde.");

                    return View(modelo);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Registro()
        {
            try
            {
                var modelo = new CrearUsarioCliente()
                {
                    informacionUsuario = new RegistroUsuarioCliente(),
                    informacionCliente = new CrearClienteDTo()
                };
                return View(modelo);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
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

                    if (usuario.Id_Role == (int)Rol.Cliente)
                    {
                        return RedirectToAction("Home", "Ecommerce");
                    }

                    return RedirectToAction("Index", "Ventas");
                }

                ModelState.AddModelError(string.Empty, "Credenciales no válidas");
                return View(modelo);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerRoles()
        {
            return await repositorioUsuarios.ObtenerRoles();
        }
    }
}