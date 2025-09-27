using inmobiliaria_mvc.Helpers;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using inmobiliaria_mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria_mvc.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly IRepositoryInquilino repositorio;
        private readonly IRepositoryContrato _repoContrato;
        private readonly IRepositoryPago _repoPago;
        private readonly IConfiguration _config;

        public InquilinoController(IRepositoryInquilino repo, IRepositoryContrato repoContrato, IRepositoryPago repoPago, IConfiguration config)
        {
            repositorio = repo;
            _repoContrato = repoContrato;
            _repoPago = repoPago;
            _config = config;
        }

        public ActionResult Filtrar(int page = 1, int pageSize = 10)
        {
            var tabla = ConstruirTabla(page, pageSize);
            return PartialView("_Tabla", tabla);
        }

        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var tabla = ConstruirTabla(page, pageSize);
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];

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

                var contratos = _repoContrato.ObtenerTodos()
                    .Where(c => c.IdInquilino == id && c.Estado)
                    .ToList();

                var pagos = new List<Pago>();
                foreach (var contrato in contratos)
                {
                    var pagosContrato = _repoPago.ObtenerPorContrato(contrato.Id, incluirAnulados: true)
                        .OrderBy(p => p.NumeroPago)
                        .ToList();
                    pagos.AddRange(pagosContrato);
                }

                var viewModel = new InquilinoDetalleVM
                {
                    Inquilino = inquilino,
                    Contratos = contratos,
                    Pagos = pagos
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

        public ActionResult Delete1(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                {
                    TempData["Error"] = "Inquilino no encontrado para eliminación.";
                    return RedirectToAction(nameof(Index));
                }
                return View(entidad);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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

        private TablaViewModel<Inquilino> ConstruirTabla(int page, int pageSize)
        {
            var lista = repositorio.Paginar(page, pageSize);
            var tabla = TablaHelper.MapToTablaViewModel(lista, l => new Dictionary<string, object>
            {
                { "Código", l.IdInquilino },
                { "DNI", l.Dni},
                { "Nombre", $"{l.Nombre} {l.Apellido}"},
                {"Teléfono", l.Telefono},
                {"Email", l.Email},
                { "Acciones", $@"
                    <a href='/Inquilino/Details/{l.IdInquilino}' class='btn btn-info btn-sm'>Detalles</a>
                    <a href='/Inquilino/Edit/{l.IdInquilino}' class='btn btn-warning btn-sm'>Editar</a>
                    <a href='/Inquilino/Delete/{l.IdInquilino}' class='btn btn-danger btn-sm'>Eliminar</a>
                " }
            });

            return tabla;
        }
    }
}
