using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace inmobiliaria_mvc.Controllers
{
    public class ContratoController : Controller
    {
        private readonly ILogger<ContratoController> _logger;

        private readonly IRepositoryContrato _repo;
        private readonly IRepositoryInmueble _repoInmueble;
        private readonly IRepositoryInquilino _repoInquilino;
        private readonly IConfiguration _config;

        public ContratoController(ILogger<ContratoController> logger, IRepositoryContrato repo, IRepositoryInmueble repoInmueble, IRepositoryInquilino repoInquilino, IConfiguration config)
        {
            _logger = logger;
            _repo = repo;
            _repoInmueble = repoInmueble;
            _repoInquilino = repoInquilino;
            _config = config;
        }

        //GET: CONTRATO
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            //var contrato = _repo.ObtenerTodos();
            var contrato = _repo.Paginar(page, pageSize);
            if (TempData.ContainsKey("Mensaje")) ViewBag.Mensaje = TempData["Mensaje"];
            return View(contrato);
        }

        //GET: Contrato/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.Inmueble = new SelectList(_repoInmueble.ObtenerTodos(), "Id", "Direccion");
                ViewBag.Inquilino = new SelectList(_repoInquilino.ObtenerTodos(), "IdInquilino", "NombreCompleto");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar contrato: Create()");
                throw;
            }
        }

        //POST: Contrato/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato contrato)
        {
            try
            {
                if (contrato.Fecha_inicio >= contrato.Fecha_fin)
                {
                    ModelState.AddModelError(string.Empty, "La fecha de inicio debe ser anterior a la fecha de fin.");
                }
                if (ModelState.IsValid)
                {
                    if (!_repo.ExisteSolapado(contrato.IdInmueble, contrato.Fecha_inicio, contrato.Fecha_fin))
                    {
                        _repo.Alta(contrato);
                        TempData["Mensaje"] = "Contrato creado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "El inmueble ya tiene un contrato en las fechas indicadas");
                    }
                }
                ViewBag.Inmueble = new SelectList(_repoInmueble.ObtenerTodos(), "Id", "Direccion");
                ViewBag.Inquilino = new SelectList(_repoInquilino.ObtenerTodos(), "IdInquilino", "NombreCompleto");
                return View(contrato);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex.Message:" + ex.Message);
                _logger.LogError(ex, "Error al crear contrato: Create()");
                return View(contrato);
            }
        }


        //GET: Contrato/Edit
        public ActionResult Edit(int id)
        {
            try
            {
                var contrato = _repo.ObtenerPorId(id);
                if (contrato == null)
                {
                    TempData["Error"] = "Contrato no encontrado para edición.";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Inmueble = new SelectList(_repoInmueble.ObtenerTodos(), "Id", "Direccion", contrato.IdInmueble);
                ViewBag.Inquilino = new SelectList(_repoInquilino.ObtenerTodos(), "IdInquilino", "NombreCompleto", contrato.IdInquilino);
                return View(contrato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar contrato: Edit()");
                return RedirectToAction(nameof(Index));
            }
        }

        //POST: contrato/edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Contrato contrato)
        {
            try
            {
                if (contrato.Fecha_inicio >= contrato.Fecha_fin)
                {
                    ModelState.AddModelError(string.Empty, "La fecha de inicio debe ser anterior a la fecha de fin.");
                }
                if (ModelState.IsValid)
                {
                    contrato.Id = id;
                    if (!_repo.ExisteSolapado(contrato.IdInmueble, contrato.Fecha_inicio, contrato.Fecha_fin, id))
                    {
                        _repo.Modificacion(contrato);
                        TempData["Mensaje"] = "Contrato modificado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "El inmueble ya tiene un contrato en las fechas indicadas");
                    }
                }
                ViewBag.Inmueble = new SelectList(_repoInmueble.ObtenerTodos(), "Id", "Direccion");
                ViewBag.Inquilino = new SelectList(_repoInquilino.ObtenerTodos(), "IdInquilino", "NombreCompleto");
                return View(contrato);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hubo un error al modificar el contrato.";
                _logger.LogError(ex, "Error al modificar contrato: Edit()");
                ViewBag.Inmueble = new SelectList(_repoInmueble.ObtenerTodos(), "Id", "Direccion");
                ViewBag.Inquilino = new SelectList(_repoInquilino.ObtenerTodos(), "IdInquilino", "NombreCompleto");
                return View(contrato);
            }
        }

        //GET: Contrato/Delete
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
                TempData["Error"] = "Hubo un error al eliminar el contrato.";
                _logger.LogError(ex, "Error al eliminar contrato: Delete()" + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        //POST: Contrato/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            try
            {
                _repo.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hubo un error al intentar eliminar el contrato.";
                _logger.LogError(ex, "Error al eliminar contrato: Delete()" + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}