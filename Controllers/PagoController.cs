using System.Security.Claims;
using System.Text.Json;
using inmobiliaria_mvc.Helpers;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using inmobiliaria_mvc.Services;
using inmobiliaria_mvc.ViewModels;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace inmobiliaria_mvc.Controllers
{
    public class PagoController : Controller
    {
        private readonly IRepositoryPago _repositorio;
        private readonly IRepositoryContrato _repoContrato;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IConfiguration _config;
        private readonly IAntiforgery _antiforgery;

        public PagoController(IRepositoryPago repo, IRepositoryContrato repoContrato,
            IAuditoriaService auditoriaService, IConfiguration config, IAntiforgery antiforgery)
        {
            _repositorio = repo;
            _repoContrato = repoContrato;
            _auditoriaService = auditoriaService;
            _config = config;
            _antiforgery = antiforgery;
        }

        // public ActionResult Index()
        // {
        //     var contratos = _repoContrato.ObtenerTodos();
        //     var todosLosPagos = new List<Pago>();

        //     foreach (var contrato in contratos)
        //     {
        //         var pagos = _repositorio.ObtenerPorContrato(contrato.Id, true).ToList();

        //         foreach (var pago in pagos)
        //         {
        //             pago.Contrato = contrato;
        //             todosLosPagos.Add(pago);
        //         }
        //     }

        //     if (TempData.ContainsKey("Mensaje"))
        //         ViewBag.Mensaje = TempData["Mensaje"];
        //     if (TempData.ContainsKey("Error"))
        //         ViewBag.Error = TempData["Error"];

        //     return View(todosLosPagos);
        // }

        public ActionResult Filtrar(int page = 1, int pageSize = 10)
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            ViewBag.AntiForgeryToken = tokens.RequestToken;

            var tabla = ConstruirTabla(page, pageSize);
            return PartialView("_Tabla", tabla);
        }

        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            ViewBag.AntiForgeryToken = tokens.RequestToken;

            var tabla = ConstruirTabla(page, pageSize);

            if (ViewBag.Mensaje == null && TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];

            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];

            return View(tabla);
        }


        public ActionResult Details(int id)
        {
            var pago = _repositorio.ObtenerPorId(id);
            if (pago == null)
            {
                TempData["Error"] = "Pago no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var contrato = _repoContrato.ObtenerPorId(pago.ContratoId);
            if (contrato == null)
            {
                TempData["Error"] = "Este contrato se encuentra inactivo.";
                return RedirectToAction(nameof(Index));
            }

            var inquilinoId = contrato.IdInquilino;
            var contratosInquilino = _repoContrato.ObtenerTodos()
                .Where(c => c.IdInquilino == inquilinoId)
                .ToList();
            var pagos = new List<Pago>();

            foreach (var c in contratosInquilino)
            {
                var pagosContrato = _repositorio.ObtenerPorContrato(c.Id, incluirAnulados: true)
                    .OrderBy(p => p.NumeroPago)
                    .ToList();
                foreach (var p in pagosContrato)
                {
                    p.Contrato = c;
                    pagos.Add(p);
                }
            }

            // if (!pagos.Any())
            // {
            //     TempData["Error"] = "No se encontraron pagos para este inquilino.";
            //     return RedirectToAction(nameof(Index));
            // }

            ViewBag.InquilinoNombre = $"{contrato.Inquilino?.Nombre} {contrato.Inquilino?.Apellido}";
            ViewBag.InquilinoId = inquilinoId;
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(pagos);
        }

        public ActionResult Create()
        {
            var contratos = _repoContrato.ObtenerTodos()
                .Select(c => new
                {
                    c.Id,
                    Descripcion = c.Inquilino.Nombre + " " + c.Inquilino.Apellido + " - " + c.Inmueble.Direccion
                })
                .ToList();
            ViewBag.Contrato = new SelectList(contratos, "Id", "Descripcion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                _repositorio.Alta(pago);

                //Auditoria
                int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                _auditoriaService.RegistrarCambio(
                    entidad: "Pago",
                    entidadId: pago.IdPago,
                    accion: "Alta",
                    usuarioId: usuarioId,
                    datosAnteriores: "",
                    datosNuevos: pago
                );

                TempData["Id"] = pago.IdPago;
                TempData["Mensaje"] = "Pago registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            var contratos = _repoContrato.ObtenerTodos()
                .Select(c => new
                {
                    c.Id,
                    Descripcion = c.Inquilino.Nombre + " " + c.Inquilino.Apellido + " - " + c.Inmueble.Direccion
                })
                .ToList();
            ViewBag.Contrato = new SelectList(contratos, "Id", "Descripcion", pago.ContratoId);
            return View(pago);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            var pago = _repositorio.ObtenerPorId(id);
            if (pago == null)
            {
                TempData["Error"] = "Pago no encontrado para edición.";
                return RedirectToAction(nameof(Index));
            }

            var contratos = _repoContrato.ObtenerTodos()
                .Select(c => new
                {
                    c.Id,
                    Descripcion = c.Inquilino.Nombre + " " + c.Inquilino.Apellido + " - " + c.Inmueble.Direccion
                })
                .ToList();
            ViewBag.Contrato = new SelectList(contratos, "Id", "Descripcion", pago.ContratoId);
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id, Pago pago)
        {
            if (id != pago.IdPago)
            {
                TempData["Error"] = "ID de pago no coincide.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var pagoExistente = _repositorio.ObtenerPorId(id);
                if (pagoExistente == null)
                {
                    TempData["Error"] = "Pago no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var pagoAnterior = JsonSerializer.Deserialize<Pago>(JsonSerializer.Serialize(pagoExistente));

                pagoExistente.Detalle = pago.Detalle;
                _repositorio.Modificacion(pagoExistente, esRegistroReal: false);

                //Auditoria
                int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                _auditoriaService.RegistrarCambio(
                    entidad: "Pago",
                    entidadId: pago.IdPago,
                    accion: "Modificación",
                    usuarioId: usuarioId,
                    datosAnteriores: pagoAnterior,
                    datosNuevos: pago
                );

                TempData["Mensaje"] = "Detalle actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al actualizar el pago.";
                var contratos = _repoContrato.ObtenerTodos()
                    .Select(c => new
                    {
                        c.Id,
                        Descripcion = c.Inquilino.Nombre + " " + c.Inquilino.Apellido + " - " + c.Inmueble.Direccion
                    })
                    .ToList();
                ViewBag.Contrato = new SelectList(contratos, "Id", "Descripcion", pago.ContratoId);
                return View(pago);
            }
        }

        // public ActionResult Delete(int id)
        // {
        //     _repositorio.Baja(id);
        //     TempData["Mensaje"] = "Pago eliminado correctamente.";
        //     return RedirectToAction(nameof(Index));
        // }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult DeletePost(int id)
        {
            _repositorio.Baja(id);
            TempData["Mensaje"] = "Pago eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrar(int contratoId, int numeroPago, int idInquilino, string returnUrl,
            string? detalle = null)
        {
            var contrato = _repoContrato.ObtenerPorId(contratoId);
            if (contrato == null)
            {
                TempData["Error"] = "Contrato no encontrado.";
                return RedirectToAction("Index", "Inquilino");
            }

            var pagos = _repositorio.ObtenerPorContrato(contratoId, incluirAnulados: true);
            var pagoPendiente = pagos.FirstOrDefault(p => p.NumeroPago == numeroPago && p.Estado);

            if (pagoPendiente == null)
            {
                TempData["Error"] = $"No hay pago pendiente para el mes {numeroPago} en el contrato {contratoId}.";
                return RedirectToAction("Details", "Inquilino", new { id = idInquilino });
            }

            pagoPendiente.Detalle = detalle ?? (pagoPendiente.EsMulta
                ? $"Multa por terminación anticipada - Pagada ({DateTime.Now:dd/MM/yyyy})"
                : $"Mes {numeroPago} - Pagado ({DateTime.Now:dd/MM/yyyy})");
            pagoPendiente.FechaPago = DateTime.Now;

            _repositorio.Modificacion(pagoPendiente, esRegistroReal: true);

            if (pagoPendiente.EsMulta)
            {
                contrato.MultaCalculada = null;
                _repoContrato.Modificacion(contrato);
                TempData["Mensaje"] = "Multa registrada como pagada correctamente.";
            }
            else
            {
                var siguienteId = _repositorio.CrearSiguientePagoSiAplica(contratoId, numeroPago, contrato.Monto,
                    contrato.Fecha_inicio, contrato.Fecha_fin);
                TempData["Mensaje"] = siguienteId.HasValue
                    ? $"Pago registrado correctamente. Siguiente pago creado (Mes {numeroPago + 1})."
                    : "Pago registrado correctamente. Contrato completado.";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        private TablaViewModel<PagoVM> ConstruirTabla(int page, int pageSize)
        {
            var pagos = _repositorio.Paginar(page, pageSize);

            var token = ViewBag.AntiForgeryToken as string;
            var registrarUrl = Url.Action("Registrar", "Pago");

            var tabla = TablaHelper.MapToTablaViewModel(pagos, p => new Dictionary<string, object>
            {
                { "Id", p.Pago.IdPago },
                { "Contrato", p.Pago.ContratoId },
                { "Inmueble", p.DireccionInmueble ?? "-" },
                { "Número de pago", p.Pago.NumeroPago },
                { "Fecha esperada", p.Pago.FechaEsperada.ToString("dd/MM/yyyy") },
                {
                    "Fecha de pago",
                    p.Pago.FechaPago.HasValue
                        ? p.Pago.FechaPago.Value.ToString("dd/MM/yyyy")
                        : "<span class='badge bg-warning text-dark'>Pendiente</span>"
                },
                { "Importe", p.Pago.Importe.ToString("C") },
                { "Detalle", p.Pago.Detalle ?? "-" },
                {
                    "Acciones", $@"
            {(p.Pago.Estado && !p.Pago.FechaPago.HasValue ? $@"
                <form method='post' action='{registrarUrl}' style='display:inline;'>
                    <input type='hidden' name='contratoId' value='{p.Pago.ContratoId}' />
                    <input type='hidden' name='numeroPago' value='{p.Pago.NumeroPago}' />
                    <input type='hidden' name='idInquilino' value='{p.IdInquilino}' />
                    <input type='hidden' name='__RequestVerificationToken' value='{token ?? ""}' />
                    <input type='hidden' name='returnUrl' value='/Pago' />
                    <button type='submit' class='btn btn-primary btn-sm'>Registrar Pago</button>
                </form>" : "")}
            {BotonHelper.BotonDetalles("Pago", p.Pago.IdPago)}
            {BotonHelper.BotonEditar("Pago", p.Pago.IdPago)}
            {BotonHelper.BotonEliminar("Pago", p.Pago.IdPago, $"Pago #{p.Pago.NumeroPago} del contrato {p.Pago.ContratoId}")}
        "
                }
            });

            return tabla;
        }
    }
}