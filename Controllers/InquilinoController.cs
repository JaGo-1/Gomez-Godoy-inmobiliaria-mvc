using inmobiliaria_mvc.Helpers;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using inmobiliaria_mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria_mvc.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly IRepositoryInquilino repositorio;
        private readonly IRepositoryContrato _repoContrato;
        private readonly IRepositoryPago _repoPago;
        private readonly IConfiguration _config;

        public InquilinoController(IRepositoryInquilino repo, IRepositoryContrato repoContrato,
            IRepositoryPago repoPago, IConfiguration config)
        {
            repositorio = repo;
            _repoContrato = repoContrato;
            _repoPago = repoPago;
            _config = config;
        }

        public ActionResult Filtrar(int page = 1, int pageSize = 10, string? termino = null)
        {
            var tabla = ConstruirTabla(page, pageSize, termino);
            return PartialView("_Tabla", tabla);
        }

        public ActionResult Index(int page = 1, int pageSize = 10, string? termino = null)
        {
            var tabla = ConstruirTabla(page, pageSize, termino);
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];

            ViewData["Termino"] = termino;

            return View(tabla);
        }

        public ActionResult Details(int id)
        {
            try
            {
                var inquilino = repositorio.ObtenerPorId(id);
                if (inquilino == null)
                {
                    TempData["Error"] = "Inquilino no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var contratos = _repoContrato.ObtenerContratosPorInquilino(id)
                    .ToList();

                foreach (var contrato in contratos)
                {
                    contrato.Pagos = _repoPago.ObtenerPorContrato(contrato.Id, incluirAnulados: true)
                        .OrderBy(p => p.NumeroPago)
                        .ToList();

                    foreach (var pago in contrato.Pagos)
                    {
                        pago.Contrato = contrato;
                    }
                }

                var viewModel = new InquilinoDetalleVM
                {
                    Inquilino = inquilino,
                    Contratos = contratos,
                };

                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al obtener detalles del inquilino: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino inquilino)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(inquilino);
                    TempData["Id"] = inquilino.IdInquilino;
                    TempData["Mensaje"] = "Inquilino creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(inquilino);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hubo un error al crear el inquilino.";
                return View(inquilino);
            }
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                {
                    TempData["Error"] = "Inquilino no encontrado para edición.";
                    return RedirectToAction(nameof(Index));
                }

                return View(entidad);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id, Inquilino entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entidad.IdInquilino = id;
                    repositorio.Modificacion(entidad);
                    TempData["Mensaje"] = "Datos guardados correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(entidad);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hubo un error al editar el inquilino.";
                return View(entidad);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hubo un error al eliminar el inquilino.";
                return RedirectToAction(nameof(Index));
            }
        }

        private TablaViewModel<Inquilino> ConstruirTabla(int page, int pageSize, string? termino)
        {
            var lista = repositorio.Paginar(page, pageSize, termino);
            var tabla = TablaHelper.MapToTablaViewModel(lista, l => new Dictionary<string, object>
            {
                { "Código", l.IdInquilino },
                { "DNI", l.Dni },
                { "Nombre", $"{l.Nombre} {l.Apellido}" },
                { "Teléfono", l.Telefono },
                { "Email", l.Email },
                {
                    "Acciones", $@"
                    {BotonHelper.BotonDetalles("Inquilino", l.IdInquilino)}
                    {BotonHelper.BotonEditar("Inquilino", l.IdInquilino)}
                    {BotonHelper.BotonEliminar("Inquilino", l.IdInquilino, $"Inquilino {l.Nombre} {l.Apellido}")}
                "
                }
            });

            return tabla;
        }
    }
}