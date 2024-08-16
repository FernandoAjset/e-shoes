using LCDE.Models;
using LCDE.Models.Enums;
using LCDE.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LCDE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISesionServicio sesionServicio;
        private readonly SignInManager<Usuario> signInManager;

        public HomeController(ILogger<HomeController> logger,
            ISesionServicio sesionServicio,
            SignInManager<Usuario> signInManager
            )
        {
            _logger = logger;
            this.sesionServicio = sesionServicio;
            this.signInManager = signInManager;
        }

        public IActionResult Index()
        {
            // validar si no hay sesion dirigir a login
            if (!signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Login", "Auth");
            }
            if (sesionServicio.ObtenerRolUsuario() == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (int.Parse(sesionServicio.ObtenerRolUsuario()) == (int)Rol.Cliente)
            {
                return RedirectToAction("Home", "Ecommerce");
            }

            return RedirectToAction("Index", "Ventas");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult NoEncontrado()
        {
            return View();
        }
    }
}