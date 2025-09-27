using inmobiliaria_mvc.Helpers;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using inmobiliaria_mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace inmobiliaria_mvc.Controllers;

public class InmuebleController : Controller
{
    private readonly IRepositoryInmueble repositorio;
    private readonly IRepositoryContrato repoContrato;
    private readonly IRepositoryPropietario repoPropietario;

    public InmuebleController(IRepositoryInmueble repositorio, IRepositoryContrato repoContrato, IRepositoryPropietario repoPropietario)
    {
        this.repositorio = repositorio;
        this.repoContrato = repoContrato;
        this.repoPropietario = repoPropietario;
    }

    public ActionResult Filtrar(int page = 1, int pageSize = 10)
    {
        var tabla = ConstruirTabla(page, pageSize);
        return PartialView("_Tabla", tabla);
    }

    public ActionResult Index(int page = 1, int pageSize = 5)
    {
        var tabla = ConstruirTabla(page, pageSize);

        if (TempData.ContainsKey("Id"))
            ViewBag.Id = TempData["Id"];
        if (TempData.ContainsKey("Mensaje"))
            ViewBag.Mensaje = TempData["Mensaje"];

        return View(tabla);
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

    public ActionResult Details(int id)
    {
        try
        {
            var inmueble = repositorio.ObtenerPorId(id);
            var contratos = repoContrato.ObtenerContratosPorInmueble(id);

            var model = new InmuebleDetalleVM
            {
                Inmueble = inmueble,
                Contratos = contratos
            };

            return View(model);
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Ocurrió un error al obtener los detalles del inmueble.";
            return RedirectToAction(nameof(Index));
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

    private TablaViewModel<Inmueble> ConstruirTabla(int page, int pageSize)
    {
        var lista = repositorio.Paginar(page, pageSize);

        var tabla = TablaHelper.MapToTablaViewModel(lista, l => new Dictionary<string, object>
        {
            { "Código", l.Id },
            { "Dirección", l.Direccion },
            { "Precio", l.Precio },
            { "Ambientes", l.Ambientes },
            { "Estado", l.Estado
                ? "<span class='badge bg-success'>Disponible</span>"
                : "<span class='badge bg-danger'>Inactivo</span>" },
            { "Propietario", l.Propietario?.NombreCompleto },
            { "Acciones", $@"
                <a href='/Inmueble/Details/{l.Id}' class='btn btn-info btn-sm'>Detalles</a>
                <a href='/Inmueble/Edit/{l.Id}' class='btn btn-warning btn-sm'>Editar</a>
                <a href='/Inmueble/Delete/{l.Id}' class='btn btn-danger btn-sm'>Eliminar</a>
            " }
        });

        return tabla;
    }
}
