using inmobiliaria_mvc.Helpers;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using inmobiliaria_mvc.ViewModels;
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

        public ActionResult Filtrar(int page = 1, int pageSize = 10, bool? disponible = null, int? plazo = null)
        {
            try
            {
                var contratos = _repo.Paginar(page, pageSize, disponible, plazo);

                var tabla = TablaHelper.MapToTablaViewModel(contratos, c => new Dictionary<string, object>
                {
                    { "Código", c.Id },
                    { "Dirección", c.Inmueble.Direccion },
                    { "Inquilino", $"{c.Inquilino.Nombre} {c.Inquilino.Apellido}" },
                    { "Monto", c.Monto },
                    { "Fecha de inicio", c.Fecha_inicio.ToString("dd/MM/yyyy") },
                    { "Fecha de fin", c.Fecha_fin.ToString("dd/MM/yyyy") },
                    { "Acciones", $@"
                        <a href='/Contrato/Details/{c.Id}' class='btn btn-info btn-sm'>Detalles</a>
                        <a href='/Contrato/Edit/{c.Id}' class='btn btn-warning btn-sm'>Editar</a>
                        <a href='/Contrato/Delete/{c.Id}' class='btn btn-danger btn-sm'>Eliminar</a>
                    " }
                });

                ViewData["Disponible"] = disponible;

                return PartialView("_Tabla", tabla);
            }
            catch (Exception e)
            {
                Console.WriteLine("error" + e.Message);
                throw;
            }
        }

        public ActionResult Index(int page = 1, int pageSize = 10, bool? disponible = null, int? plazo = null)
        {
            var contratos = _repo.Paginar(page, pageSize, disponible, plazo);
            var tabla = TablaHelper.MapToTablaViewModel(contratos, c => new Dictionary<string, object>
            {
                { "Código", c.Id },
                { "Dirección", c.Inmueble.Direccion },
                { "Inquilino", $"{c.Inquilino.Nombre} {c.Inquilino.Apellido}" },
                { "Monto", c.Monto },
                { "Fecha de inicio", c.Fecha_inicio.ToString("dd/MM/yyyy") },
                { "Fecha de fin", c.Fecha_fin.ToString("dd/MM/yyyy") },
                { "Acciones", $@"
                    <a href='/Contrato/Details/{c.Id}' class='btn btn-info btn-sm'>Detalles</a>
                    <a href='/Contrato/Edit/{c.Id}' class='btn btn-warning btn-sm'>Editar</a>
                    <a href='/Contrato/Delete/{c.Id}' class='btn btn-danger btn-sm'>Eliminar</a>
                " }
            });

            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];

            ViewData["Disponible"] = disponible;

            return View(tabla);
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