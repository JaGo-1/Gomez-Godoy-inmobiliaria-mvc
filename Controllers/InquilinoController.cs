using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria_mvc.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly IRepositoryInquilino repositorio;
        private readonly IConfiguration _config;

        public InquilinoController(IRepositoryInquilino repo, IConfiguration config)
        {
            this.repositorio = repo;
            this._config = config;
        }

        public ActionResult Index()
        {
            try
            {
                var lista = repositorio.ObtenerTodos();
                ViewBag.Id = TempData["Id"];
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                return View(lista);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                throw;
            }
        }
        
        public ActionResult Details(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                {
                    TempData["Error"] = "Inquilino no encontrado.";
                    return RedirectToAction(nameof(Index));
                }
                return View(entidad);
            }
            catch (Exception ex)
            {
                throw;
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

        public ActionResult Delete(int id)
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
    }
}
