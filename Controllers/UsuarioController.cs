using System.Security.Claims;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria_mvc.Controllers;

public class UsuarioController : Controller
{
    private readonly IConfiguration configuration;
    private readonly IWebHostEnvironment environment;
    private readonly IRepositoryUsuario repository;

    public UsuarioController(IConfiguration configuration,
        IWebHostEnvironment environment,
        IRepositoryUsuario repository)
    {
        this.configuration = configuration;
        this.environment = environment;
        this.repository = repository;
    }

    // GET: Usuario
    [Authorize(Policy = "Administrador")]
    public IActionResult Index()
    {
        var usuarios = repository.ObtenerTodos();
        return View(usuarios);
    }

    // GET: Usuario/Details/5
    [Authorize(Policy = "Administrador")]
    public ActionResult Details(int id)
    {
        var e = repository.ObtenerPorId(id);
        return View(e);
    }

    // GET: Usuario/Create
    [Authorize(Policy = "Administrador")]
    public ActionResult Create()
    {
        ViewBag.Roles = Usuario.ObtenerRoles();
        return View();
    }

    // POST: Usuarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Administrador")]
    public ActionResult Create(Usuario u)
    {
        if (!ModelState.IsValid)
            return View();
        try
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: u.Password,
                salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
            u.Password = hashed;
            u.Rol = User.IsInRole("Administrador") ? u.Rol : (int)enRoles.Empleado;
            var nbreRnd = Guid.NewGuid();
            int res = repository.Alta(u);
            if (u.AvatarFile != null && u.Id > 0)
            {
                string wwwPath = environment.WebRootPath;
                string path = Path.Combine(wwwPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                string pathCompleto = Path.Combine(path, fileName);
                u.Avatar = Path.Combine("/Uploads", fileName);
                using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                {
                    u.AvatarFile.CopyTo(stream);
                }

                repository.Modificacion(u);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }
    }

    // GET: Usuarios/Edit/5
    [Authorize]
    public ActionResult Perfil()
    {
        ViewData["Title"] = "Mi perfil";
        var u = repository.ObtenerPorEmail(User.Identity.Name);
        ViewBag.Roles = Usuario.ObtenerRoles();
        return ViewBag("Edit", u);
    }

    // GET: Usuarios/Edit/5
    [Authorize(Policy = "Administrador")]
    public ActionResult Edit(int id)
    {
        ViewData["Title"] = "Editar usuario";
        var u = repository.ObtenerPorId(id);
        ViewBag.Roles = Usuario.ObtenerRoles();
        return View(u);
    }

// POST: Usuarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Edit(int id, Usuario u)
    {
        var vista = nameof(Edit);
        try
        {
            var usuarioDb = repository.ObtenerPorId(id);

            if (!User.IsInRole("Administrador"))
            {
                vista = nameof(Perfil);
                var usuarioActual = repository.ObtenerPorEmail(User.Identity.Name);
                if (usuarioActual.Id != id)
                    return RedirectToAction(nameof(Index), "Home");
            }

            if (ModelState.IsValid)
            {
                usuarioDb.Nombre = u.Nombre;
                usuarioDb.Apellido = u.Apellido;
                usuarioDb.Email = u.Email;
                
                if (User.IsInRole("Administrador"))
                {
                    usuarioDb.Rol = u.Rol;
                }

                if (!string.IsNullOrEmpty(u.Password))
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: u.Password,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    usuarioDb.Password = hashed;
                }
                
                if (u.AvatarFile != null)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    usuarioDb.Avatar = Path.Combine("/Uploads", fileName);
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                }
                
                repository.Modificacion(usuarioDb);
                TempData["Mensaje"] = "Datos guardados correctamente.";
            }
            else
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(vista, usuarioDb);
            }

            return RedirectToAction(vista);
        }
        catch (Exception)
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            TempData["Error"] = "Ocurrió un error al intentar guardar los datos.";
            return View(vista, repository.ObtenerPorId(id));
        }
    }

    // GET: Usuarios/Delete/5
    [Authorize(Policy = "Administrador")]
    public ActionResult Delete(int id)
    {
        throw new NotImplementedException();
    }

    // POST: Usuarios/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Administrador")]
    public ActionResult Delete(int id, Usuario usuario)
    {
        try
        {
            var ruta = Path.Combine(environment.WebRootPath, "Uploads",
                $"avatar_{id}" + Path.GetExtension(usuario.Avatar));
            if (System.IO.File.Exists(ruta))
                System.IO.File.Delete(ruta);
            repository.Baja(id);
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    [Authorize]
    public IActionResult Avatar()
    {
        var u = repository.ObtenerPorEmail(User.Identity.Name);
        string fileName = "avatar_" + u.Id + Path.GetExtension(u.Avatar);
        string wwwPath = environment.WebRootPath;
        string path = Path.Combine(wwwPath, "Uploads");
        string pathCompleto = Path.Combine(path, fileName);

        byte[] fileBytes = System.IO.File.ReadAllBytes(pathCompleto);
        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
    }

    [Authorize]
    public string AvatarBase64()
    {
        var u = repository.ObtenerPorEmail(User.Identity.Name);
        string fileName = "avatar_" + u.Id + Path.GetExtension(u.Avatar);
        string wwwPath = environment.WebRootPath;
        string path = Path.Combine(wwwPath, "Uploads");
        string pathCompleto = Path.Combine(path, fileName);

        byte[] fileBytes = System.IO.File.ReadAllBytes(pathCompleto);
        return Convert.ToBase64String(fileBytes);
    }

    [Authorize]
    [HttpPost("[controller]/[action]/{fileName}")]
    public IActionResult FromBase64([FromBody] string imagen, [FromRoute] string fileName)
    {
        string wwwPath = environment.WebRootPath;
        string path = Path.Combine(wwwPath, "Uploads");
        string pathCompleto = Path.Combine(path, fileName);
        var bytes = Convert.FromBase64String(imagen);
        System.IO.File.WriteAllBytes(pathCompleto, bytes);
        return Ok();
    }

    [Authorize]
    public ActionResult Foto()
    {
        try
        {
            var u = repository.ObtenerPorEmail(User.Identity.Name);
            var stream = System.IO.File.Open(
                Path.Combine(environment.WebRootPath, u.Avatar.Substring(1)),
                FileMode.Open,
                FileAccess.Read);
            var ext = Path.GetExtension(u.Avatar);
            return new FileStreamResult(stream, $"image/{ext.Substring(1)}");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [Authorize]
    public ActionResult Datos()
    {
        try
        {
            var u = repository.ObtenerPorEmail(User.Identity.Name);
            string buffer = "Nombre;Apellido;Email" + Environment.NewLine +
                            $"{u.Nombre};{u.Apellido};{u.Email}";
            var stream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(buffer));
            var res = new FileStreamResult(stream, "text/plain");
            res.FileDownloadName = "Datos.csv";
            return res;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [AllowAnonymous]
    // GET: Usuarios/Login/
    public ActionResult LoginModal()
    {
        return PartialView("_LoginModal", new LoginView());
    }

    [AllowAnonymous]
    // GET: Usuarios/Login/
    public ActionResult Login(string returnUrl)
    {
        TempData["returnUrl"] = returnUrl;
        return View();
    }

    // POST: Usuarios/Login/
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginView login)
    {
        try
        {
            var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string)
                ? "/Home"
                : TempData["returnUrl"].ToString();
            if (ModelState.IsValid)
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: login.Password,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                var e = repository.ObtenerPorEmail(login.Usuario);
                if (e == null || e.Password != hashed)
                {
                    ModelState.AddModelError("", "El email o la clave no son correctos");
                    TempData["returnUrl"] = returnUrl;
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, e.Email),
                    new Claim("FullName", e.Nombre + " " + e.Apellido),
                    new Claim(ClaimTypes.Role, e.RolNombre),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
                TempData.Remove("returnUrl");
                return Redirect(returnUrl);
            }

            TempData["returnUrl"] = returnUrl;
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }

    // GET: /salir
    [Route("salir", Name = "logout")]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
    
    [AllowAnonymous]
    public string GetTestHash(string password)
    {
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 1000,
            numBytesRequested: 256 / 8));
        return hashed;
    }
}