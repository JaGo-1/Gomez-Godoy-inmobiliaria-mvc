using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using inmobiliaria_mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria_mvc.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly ILogger<PropietarioController> _logger;

        private readonly IRepositoryPropietario _repo;
        private readonly IRepositoryInmueble _repoInmueble;
        private readonly IConfiguration _config;

        public PropietarioController(ILogger<PropietarioController> logger, IRepositoryPropietario repo, IRepositoryInmueble repoInmueble, IConfiguration config)
        {
            _logger = logger;
            _repo = repo;
            _repoInmueble = repoInmueble;
            _config = config;
        }

        // GET: Propietario
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            //var propietarios = _repo.ObtenerTodos();
            var propietarios = _repo.Paginar(page, pageSize);
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
                _logger.LogError(ex, "Error al crear propietario: Create()");
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

        // GET: Propietario/Details/:id
        public ActionResult Details(int id)
        {
            try
            {
                var propietario = _repo.ObtenerPorId(id);
                var inmueles = _repoInmueble.ObtenerPorPropietario(id);

                var model = new PropietarioDetalleVM
                {
                    Propietario = propietario,
                    Inmuebles = inmueles
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al obtener los detalles del propietario.";
                return RedirectToAction(nameof(Index));
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
                _logger.LogError(e, "Error al eliminar propietario: Delete()");
                throw;
            }
        }
    }
}