using System.Security.Claims;
using System.Text.Json;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using inmobiliaria_mvc.Services;
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

        public PagoController(IRepositoryPago repo, IRepositoryContrato repoContrato, IAuditoriaService auditoriaService, IConfiguration config)
        {
            _repositorio = repo;
            _repoContrato = repoContrato;
            _auditoriaService = auditoriaService;
            _config = config;
        }

        public ActionResult Index()
        {
            var contratos = _repoContrato.ObtenerTodos();
            var todosLosPagos = new List<Pago>();

            foreach (var contrato in contratos)
            {
                var pagos = _repositorio.ObtenerPorContrato(contrato.Id, true).ToList();

                foreach (var pago in pagos)
                {
                    pago.Contrato = contrato;
                    todosLosPagos.Add(pago);
                }
            }

            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];

            return View(todosLosPagos);
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
                TempData["Error"] = "Contrato no encontrado.";
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

            if (!pagos.Any())
            {
                TempData["Error"] = "No se encontraron pagos para este inquilino.";
                return RedirectToAction(nameof(Index));
            }

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
                .Select(c => new { c.Id, Descripcion = c.Inquilino.Nombre + " " + c.Inquilino.Apellido + " - " + c.Inmueble.Direccion })
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
                .Select(c => new { c.Id, Descripcion = c.Inquilino.Nombre + " " + c.Inquilino.Apellido + " - " + c.Inmueble.Direccion })
                .ToList();
            ViewBag.Contrato = new SelectList(contratos, "Id", "Descripcion", pago.ContratoId);
            return View(pago);
        }

        public ActionResult Edit(int id)
        {
            var pago = _repositorio.ObtenerPorId(id);
            if (pago == null)
            {
                TempData["Error"] = "Pago no encontrado para edición.";
                return RedirectToAction(nameof(Index));
            }
            var contratos = _repoContrato.ObtenerTodos()
                .Select(c => new { c.Id, Descripcion = c.Inquilino.Nombre + " " + c.Inquilino.Apellido + " - " + c.Inmueble.Direccion })
                .ToList();
            ViewBag.Contrato = new SelectList(contratos, "Id", "Descripcion", pago.ContratoId);
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    .Select(c => new { c.Id, Descripcion = c.Inquilino.Nombre + " " + c.Inquilino.Apellido + " - " + c.Inmueble.Direccion })
                    .ToList();
                ViewBag.Contrato = new SelectList(contratos, "Id", "Descripcion", pago.ContratoId);
                return View(pago);
            }
        }

        public ActionResult Delete(int id)
        {
            _repositorio.Baja(id);
            TempData["Mensaje"] = "Pago eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            _repositorio.Baja(id);
            TempData["Mensaje"] = "Pago eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrar(int contratoId, int numeroPago, string? detalle = null)
        {
            var contrato = _repoContrato.ObtenerPorId(contratoId);
            if (contrato == null)
            {
                TempData["Error"] = "Contrato no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var pagos = _repositorio.ObtenerPorContrato(contratoId, incluirAnulados: true);
            var pagoPendiente = pagos.FirstOrDefault(p => p.NumeroPago == numeroPago && p.Estado);

            if (pagoPendiente == null)
            {
                TempData["Error"] = $"No hay pago pendiente para el mes {numeroPago} en el contrato {contratoId}.";
                return RedirectToAction(nameof(Index));
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
                var siguienteId = _repositorio.CrearSiguientePagoSiAplica(contratoId, numeroPago, contrato.Monto, contrato.Fecha_inicio, contrato.Fecha_fin);
                TempData["Mensaje"] = siguienteId.HasValue
                    ? $"Pago registrado correctamente. Siguiente pago creado (Mes {numeroPago + 1})."
                    : "Pago registrado correctamente. Contrato completado.";
            }

            return RedirectToAction(nameof(Details), new { id = pagoPendiente.IdPago });

        }
    }
}