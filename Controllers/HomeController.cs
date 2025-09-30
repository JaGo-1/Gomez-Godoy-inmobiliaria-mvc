using System.Security.Claims;
using inmobiliaria_mvc.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;

namespace inmobiliaria_mvc.Controllers
{
	public class HomeController : Controller
	{
		private readonly IRepositoryPropietario propietarios;
		private readonly IConfiguration config;

		public HomeController(IRepositoryPropietario propietarios, IConfiguration config, ILogger<HomeController> logger)
		{
			this.propietarios = propietarios;
			this.config = config;
			logger.LogInformation("HomeController creado");
		}

		public IActionResult Index()
		{
			ViewBag.Titulo = "Página de Inicio";
			List<string> clientes = propietarios.ObtenerTodos().Select(x => x.Nombre + " " + x.Apellido).ToList();
			return View(clientes);
		}

		[Authorize]
		public ActionResult Seguro()
		{
			var identity = (ClaimsIdentity)User.Identity;
			IEnumerable<Claim>? claims = identity?.Claims;
			return View(claims);
		}

		[Authorize(Policy = "Administrador")]
		public ActionResult Admin()
		{
			return View();
		}

		public ActionResult Restringido()
		{
			return View();
		}

		[Authorize]
		public async Task<ActionResult> CambiarClaim()
		{
			var identity = (ClaimsIdentity)User.Identity;
			identity.RemoveClaim(identity.FindFirst("FullName"));
			identity.AddClaim(new Claim("FullName", "Cosme Fulanito"));
			await HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(identity));
			return Redirect(nameof(Seguro));
		}

		public IActionResult Fecha(int anio, int mes, int dia)
		{
			DateTime dt = new DateTime(anio, mes, dia);
			ViewBag.Fecha = dt;
			return View();
		}

		public IActionResult Ruta(string valor)
		{
			DateTime posibleFecha;
			if (DateTime.TryParse(valor, out posibleFecha))
			{
				ViewBag.Valor = "Escribiste una fecha: " + posibleFecha.ToString("dd/MM/yyyy");
			}
			else
			{
				ViewBag.Valor = valor;
			}
			return View();
		}

		public IActionResult Error(int codigo)
		{
			var res = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
			switch (codigo)
			{
				case 404:
					ViewBag.Ruta = res.OriginalPath;
					break;
				default:
					break;
			}
			return View();
		}

		public IActionResult Chat()
		{
			return View();
		}

		public IActionResult ChatSeguro()
		{
			return View();
		}
	}
}