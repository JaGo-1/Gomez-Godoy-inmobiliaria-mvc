using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using inmobiliaria_mvc.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace inmobiliaria_mvc.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly IRepositoryPropietario _repo;
        private readonly IConfiguration _config;

        public PropietarioController(IRepositoryPropietario repo, IConfiguration config)
        {
            this._repo = repo;
            this._config = config;
        }

        // GET: Propietario
        public ActionResult Index()
        {
            var propietarios = _repo.ObtenerTodos();
            if (TempData.ContainsKey("Mensaje")) ViewBag.Mensaje = TempData["Mensaje"];
            return View(propietarios);

        }

        //GET: Propietarios/Obtener/:id
        public IActionResult Obtener(int id)
        {
            try
            {
                var res = _repo.ObtenerPorId(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //GET: Propietario/Create
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //POST: Propietario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repo.Alta(propietario);
                    TempData["Id"] = propietario.Id;
                    TempData["Mensaje"] = "Propietario creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(propietario);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(propietario);
            }
        }

        // GET: Propietario/Edit/:id
        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                var propietario = _repo.ObtenerPorId(id);
                if (propietario == null)
                {
                    TempData["Error"] = "No se ha encontrado el propietario.";
                    return RedirectToAction(nameof(Index));
                }
                return View(propietario);
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }
        }


        // POST: Propietario/Edit/:id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    propietario.Id = id;
                    _repo.Modificacion(propietario);
                    TempData["Mensaje"] = "Datos guardados correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                return View(propietario);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(propietario);
            }
        }


        //POST: Propietarios/Edit/:id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(int id, ChangePassViewModel cpvm)
        {
            Propietario propietario = null;
            try
            {
                propietario = _repo.ObtenerPorId(id);
                var pass = Convert.ToBase64String(KeyDerivation.Pbkdf2
                    (
                        password: cpvm.PrevPass ?? "",
                        salt: System.Text.Encoding.ASCII.GetBytes(_config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8
                    ));

                if (propietario.Clave != pass)
                {
                    TempData["Error"] = "Clave incorrecta";
                    return RedirectToAction("Edit", new { id = id });
                }

                if (ModelState.IsValid)
                {
                    propietario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: cpvm.NewPass,
                        salt: System.Text.Encoding.ASCII.GetBytes(_config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8
                    ));
                    _repo.Modificacion(propietario);
                    TempData["Message"] = "Contraseña actualizada";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (ModelStateEntry modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            TempData["Error"] += error.ErrorMessage + "\n";
                        }
                    }
                    return RedirectToAction("Edit", new { id = id });
                }
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
                TempData["StackTrace"] = e.StackTrace;
                return RedirectToAction("Edit", new { id = id });
            }
        }

        //GET: Propietario/Delete/:id
        public ActionResult Delete(int id)
        {
            try
            {
                _repo.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hubo un error al eliminar el propietario.";
                return RedirectToAction(nameof(Index));
            }
        }

        //POST: Propietario/Delete/:id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Propietario propietario)
        {
            try
            {
                _repo.Baja(id);
                TempData["Mensaje"] = "Eliminado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}