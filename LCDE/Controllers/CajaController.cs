using LCDE.Models;
using LCDE.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LCDE.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class CajaController : Controller
    {
        private readonly SignInManager<Usuario> signInManager;
        private readonly RepositorioRegistroCaja repositorioRegistroCaja;

        public CajaController(
             SignInManager<Usuario> signInManager,
             RepositorioRegistroCaja repositorioRegistroCaja
            )
        {
            this.signInManager = signInManager;
            this.repositorioRegistroCaja = repositorioRegistroCaja;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(RegistroCaja registroCaja)
        {
            // Validación de las validaciones del modelo.
            if (!ModelState.IsValid)
            {
                return View(registroCaja);
            }
            registroCaja.IdCaja = 1;
            registroCaja.IdUsuario = int.Parse(signInManager.UserManager.GetUserId(User));


            // Enviar los datos al repositorio para grabar en base de datos, si se crea el registro se obtiene el nuevo Id.
            if (await repositorioRegistroCaja.CrearRegistroCaja(registroCaja) > 0)
            {
                // Si la operación es correcta se retorna al index.
                return RedirectToAction("Index");
            }
            else
            {
                // Si ocurre un error se retorna a la vista de error.
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
