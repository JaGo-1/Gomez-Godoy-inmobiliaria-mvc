using System.Security.Claims;
using inmobiliaria_mvc.Helpers;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Models.Filtros;
using inmobiliaria_mvc.Repository;
using inmobiliaria_mvc.Services;
using inmobiliaria_mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IRepositoryPago _repoPago;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IConfiguration _config;

        public ContratoController(ILogger<ContratoController> logger, IRepositoryContrato repo,
            IRepositoryInmueble repoInmueble, IRepositoryInquilino repoInquilino, IRepositoryPago repoPago,
            IAuditoriaService auditoriaService, IConfiguration config)
        {
            _logger = logger;
            _repo = repo;
            _repoInmueble = repoInmueble;
            _repoInquilino = repoInquilino;
            _repoPago = repoPago;
            _auditoriaService = auditoriaService;
            _config = config;
        }

        //GET: CONTRATO

        public ActionResult Filtrar
        (
            int page = 1,
            int pageSize = 10,
            bool? disponible = null,
            int? plazo = null,
            DateTime? desde = null,
            DateTime? hasta = null,
            string? inquilino = null,
            string? direccion = null
        )
        {
            try
            {
                var filtro = new ContratoFiltro
                {
                    Disponible = disponible,
                    Plazo = plazo,
                    Desde = desde,
                    Hasta = hasta,
                    Inquilino = inquilino,
                    Direccion = direccion
                };

                var tabla = ConstruirTabla(page, pageSize, filtro);
                ViewData["Disponible"] = disponible;

                return PartialView("_Tabla", tabla);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                throw;
            }
        }

        public ActionResult Index(
            int page = 1,
            int pageSize = 10,
            bool? disponible = null,
            int? plazo = null,
            DateTime? desde = null,
            DateTime? hasta = null,
            string? inquilino = null,
            string? direccion = null
        )
        {
            var filtro = new ContratoFiltro
            {
                Disponible = disponible,
                Plazo = plazo,
                Desde = desde,
                Hasta = hasta,
                Inquilino = inquilino,
                Direccion = direccion
            };

            var tabla = ConstruirTabla(page, pageSize, filtro);

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

                if (contrato.Fecha_inicio <= DateTime.Today && contrato.Fecha_fin >= DateTime.Today)
                {
                    contrato.Estado = true;
                }
                else
                {
                    contrato.Estado = false;
                }

                if (ModelState.IsValid)
                {
                    if (!_repo.ExisteSolapado(contrato.IdInmueble, contrato.Fecha_inicio, contrato.Fecha_fin))
                    {
                        _repo.Alta(contrato);

                        //Auditoria
                        int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                        _auditoriaService.RegistrarCambio(
                            entidad: "Contrato",
                            entidadId: contrato.Id,
                            accion: "Alta",
                            usuarioId: usuarioId,
                            datosAnteriores: "",
                            datosNuevos: contrato
                        );

                        TempData["Mensaje"] = "Contrato creado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty,
                            "El inmueble ya tiene un contrato en las fechas indicadas");
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

        //GET:Contrato/Renovar/:id
        public ActionResult Renovar(int id)
        {
            var contrato = _repo.ObtenerPorId(id);
            if (contrato == null)
            {
                TempData["Error"] = "Contrato no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            contrato.Inquilino = _repoInquilino.ObtenerPorId(contrato.IdInquilino);
            contrato.Inmueble = _repoInmueble.ObtenerPorId(contrato.IdInmueble);

            return View("Renovar", contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Renovar(Contrato contrato)
        {
            if (contrato.Fecha_inicio >= contrato.Fecha_fin)
            {
                ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");
            }

            if (ModelState.IsValid)
            {
                if (!_repo.ExisteSolapado(contrato.IdInmueble, contrato.Fecha_inicio, contrato.Fecha_fin))
                {
                    _repo.Alta(contrato);

                    //Auditoria
                    int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    _auditoriaService.RegistrarCambio(
                        entidad: "Contrato",
                        entidadId: contrato.Id,
                        accion: "Alta",
                        usuarioId: usuarioId,
                        datosAnteriores: "",
                        datosNuevos: contrato
                    );

                    TempData["Mensaje"] = "Contrato renovado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "El inmueble ya tiene un contrato en esas fechas.");
                }
            }

            return View("Renovar", contrato);
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
                ViewBag.Inquilino = new SelectList(_repoInquilino.ObtenerTodos(), "IdInquilino", "NombreCompleto",
                    contrato.IdInquilino);
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

                        //Auditoria
                        int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                        _auditoriaService.RegistrarCambio(
                            entidad: "Contrato",
                            entidadId: contrato.Id,
                            accion: "Modificación",
                            usuarioId: usuarioId,
                            datosAnteriores: "",
                            datosNuevos: contrato
                        );

                        TempData["Mensaje"] = "Contrato modificado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty,
                            "El inmueble ya tiene un contrato en las fechas indicadas");
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

        //POST: Contrato/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
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

        public ActionResult TerminarAnticipado(int id)
        {
            var contrato = _repo.ObtenerPorId(id);
            if (contrato == null)
            {
                TempData["Error"] = "Contrato no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Details", "Inquilino", new { id = contrato.IdInquilino });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TerminarAnticipado(int Id, DateTime FechaTerminacionAnticipada, bool pagarMultaAhora)
        {
            var contrato = _repo.ObtenerPorId(Id);
            if (contrato == null)
            {
                TempData["Error"] = "Contrato no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            if (FechaTerminacionAnticipada < contrato.Fecha_inicio || FechaTerminacionAnticipada > contrato.Fecha_fin)
            {
                TempData["Error"] = "La fecha de terminación debe estar dentro del periodo del contrato original.";
                return RedirectToAction("Details", "Inquilino", new { id = contrato.IdInquilino });
            }

            try
            {
                bool exito = _repo.TerminarAnticipado(Id, FechaTerminacionAnticipada, pagarMultaAhora);

                if (exito)
                {
                    var multaImporte = _repo.CalcularMultaImporte(contrato, FechaTerminacionAnticipada);
                    string estadoPago = pagarMultaAhora ? "pagada en el momento" : "registrada como pendiente";

                    TempData["Mensaje"] =
                        $"Contrato N° {Id} terminado anticipadamente el {FechaTerminacionAnticipada:d}. Multa: {multaImporte:C}, {estadoPago}.";

                    return RedirectToAction("Details", "Inquilino", new { id = contrato.IdInquilino });
                }

                TempData["Error"] = "Error al terminar anticipadamente el contrato. Verifique las fechas.";
                return RedirectToAction("Details", "Inquilino", new { id = contrato.IdInquilino });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al terminar anticipadamente el contrato: {ex.Message}";
                _logger.LogError(ex, $"Error al terminar anticipadamente contrato {Id}");
                return RedirectToAction("Details", "Inquilino", new { id = contrato.IdInquilino });
            }
        }


        // GET: Contrato/CalcularMulta
        [HttpGet]
        public JsonResult CalcularMulta(int id, DateTime fechaTerminacion)
        {
            var contrato = _repo.ObtenerPorId(id);
            if (contrato == null) return Json(new { error = "Contrato no encontrado." });

            if (fechaTerminacion < contrato.Fecha_inicio || fechaTerminacion > contrato.Fecha_fin)
                return Json(new { error = "La fecha de terminación debe estar dentro del periodo del contrato." });

            var totalMeses = _repo.CalcularMesesContrato(contrato.Fecha_inicio, contrato.Fecha_fin);
            var mesesTranscurridos = _repo.CalcularMesesTranscurridos(contrato, fechaTerminacion);
            var mesesAdeudados = _repo.CalcularMesesAdeudados(contrato, fechaTerminacion);
            var multaMeses = _repo.CalcularMultaMeses(contrato, fechaTerminacion);
            var multaImporte = _repo.CalcularMultaImporte(contrato, fechaTerminacion);

            return Json(new
            {
                totalMeses,
                mesesTranscurridos,
                mesesAdeudados,
                multaMeses,
                multaImporte
            });
        }

        private TablaViewModel<Contrato> ConstruirTabla(int page, int pageSize, ContratoFiltro filtro)
        {
            var contratos = _repo.Paginar(page, pageSize, filtro);

            var tabla = TablaHelper.MapToTablaViewModel(contratos, c => new Dictionary<string, object>
            {
                { "Código", c.Id },
                { "Dirección", c.Inmueble?.Direccion ?? "Sin dirección" },
                { "Inquilino", $"{c.Inquilino?.Nombre} {c.Inquilino?.Apellido}".Trim() },
                { "Monto", c.Monto.ToString("C") },
                { "Fecha de inicio", c.Fecha_inicio.ToString("dd/MM/yyyy") },
                { "Fecha de fin", c.Fecha_fin.ToString("dd/MM/yyyy") },
                { "Estado", c.Estado
                    ? "<span class='badge bg-success'>Vigente</span>"
                    : "<span class='badge bg-danger'>Inactivo</span>"
                },
                { "Acciones", $@"
                    <a href='/Contrato/Renovar/{c.Id}' class='btn btn-success btn-sm'>Renovar</a>
                    <a href='/Contrato/Edit/{c.Id}' class='btn btn-warning btn-sm'>Editar</a>
                    {BotonHelper.BotonEliminar(
                        "Contrato",
                        c.Id,
                        $"Contrato del inmueble {c.Inmueble?.Direccion ?? "(sin dirección)"} - Inquilino {c.Inquilino?.Nombre} {c.Inquilino?.Apellido}"
                    )}
                " }
            });

            return tabla;
        }

    }
}