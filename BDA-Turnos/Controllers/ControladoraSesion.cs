using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BDA_Turnos.Repositorio;

namespace BDA_Turnos.Controllers
{
    public class ControladoraSesion : Controller
    {
        private readonly RepositorioUsuarios _repoUsuarios;
        public ControladoraSesion(RepositorioUsuarios repoUsuarios)
        {
            _repoUsuarios = repoUsuarios;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string nombreUsuario, string clave)
        {
            try
            {
                var usuarioExiste = await _repoUsuarios.RecuperarUsuarioAsync(nombreUsuario);

                if (usuarioExiste == null)
                {
                    ViewBag.Error = "El nombre del usuario o la contraseña son incorrectos.";
                    return View();
                }

                bool esClaveValida = usuarioExiste.ValidarClave(clave);

                if (!esClaveValida)
                {
                    ViewBag.Error = "El nombre de usuario o la contraseña son incorrectos.";
                    return View();
                }

                // si es exito creo la credencial de identidad
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuarioExiste.UsuarioId.ToString()),
                    new Claim(ClaimTypes.Name, usuarioExiste.Nombre_Usuario),
                    new Claim(ClaimTypes.Role, "Usuario")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error crítico en el servidor de autenticación: {ex.Message}";
                return View();
            }
        }
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "ControladoraSesion");
        }
    }
}
