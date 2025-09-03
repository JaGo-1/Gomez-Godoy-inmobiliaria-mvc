using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace inmobiliaria_mvc.Controllers;

public class InmuebleController : Controller
{
    private readonly IRepositoryInmueble repositorio;
    private readonly IRepositoryPropietario repoPropietario;

    public InmuebleController(IRepositoryInmueble repositorio, IRepositoryPropietario repoPropietario)
    {
        this.repositorio = repositorio;
        this.repoPropietario = repoPropietario;
    }

    public ActionResult Index()
    {
        var lista = repositorio.ObtenerTodos();
        if (TempData.ContainsKey("Id"))
            ViewBag.Id = TempData["Id"];
        if (TempData.ContainsKey("Mensaje"))
            ViewBag.Mensaje = TempData["Mensaje"];
        return View(lista);
    }

    public ActionResult Create()
    {
        ViewBag.Propietarios = new SelectList(repoPropietario.ObtenerTodos(), "Id", "NombreCompleto");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Inmueble inmueble)
    {
        try
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(inmueble);
                TempData["Id"] = inmueble.Id;
                TempData["Mensaje"] = "Inmueble creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Propietarios = new SelectList(repoPropietario.ObtenerTodos(), "Id", "NombreCompleto");
            return View(inmueble);
        }
        catch
        {
            TempData["Error"] = "Hubo un error al crear el inmueble.";
            ViewBag.Propietarios = new SelectList(repoPropietario.ObtenerTodos(), "Id", "NombreCompleto");
            return View(inmueble);
        }
    }

    public ActionResult Edit(int id)
    {
        var inmueble = repositorio.ObtenerPorId(id);
        if (inmueble == null)
        {
            TempData["Error"] = "Inmueble no encontrado para edición.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Propietarios = new SelectList(repoPropietario.ObtenerTodos(), "Id", "NombreCompleto", inmueble.PropietarioId);
        return View(inmueble);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, Inmueble inmueble)
    {
        try
        {
            if (ModelState.IsValid)
            {
                inmueble.Id = id;
                repositorio.Modificacion(inmueble);
                TempData["Mensaje"] = "Datos guardados correctamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Propietarios = new SelectList(repoPropietario.ObtenerTodos(), "Id", "NombreCompleto", inmueble.PropietarioId);
            return View(inmueble);
        }
        catch
        {
            TempData["Error"] = "Hubo un error al editar el Inmueble.";
            ViewBag.Propietarios = new SelectList(repoPropietario.ObtenerTodos(), "Id", "NombreCompleto", inmueble.PropietarioId);
            return View(inmueble);
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
        catch
        {
            TempData["Error"] = "Hubo un error al eliminar el Inmueble.";
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
        catch
        {
            TempData["Error"] = "Hubo un error al eliminar el Inmueble.";
            return RedirectToAction(nameof(Index));
        }
    }
}
