using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria_mvc.Controllers;

public class ImagenesController : Controller
{
    private readonly IRepositoryImagen _repository;

    public ImagenesController(IRepositoryImagen repository)
    {
        this._repository = repository;
    }
    [HttpPost]
    public async Task<IActionResult> Alta(int id, List<IFormFile> imagenes, [FromServices] IWebHostEnvironment environment)
    {
        if (imagenes == null || imagenes.Count == 0)
            return BadRequest("No se recibieron archivos.");
        string wwwPath = environment.WebRootPath;
        string path = Path.Combine(wwwPath, "Uploads");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        path = Path.Combine(path, "Inmuebles");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        path = Path.Combine(path, id.ToString());
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        foreach (var file in imagenes)
        {
            if (file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName);
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                var rutaArchivo = Path.Combine(path, nombreArchivo);

                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                Imagen imagen = new Imagen
                {
                    InmuebleId = id,
                    Url = $"/Uploads/Inmuebles/{id}/{nombreArchivo}",
                };
                _repository.Alta(imagen);
            }
        }
        return Ok(_repository.BuscarPorInmueble(id));
    }

    // POST: Inmueble/Eliminar/5
    [HttpPost]
    public ActionResult Eliminar(int id)
    {
        try
        {
            //TODO: Eliminar el archivo físico
            var entidad = _repository.ObtenerPorId(id);
            _repository.Baja(id);
            return Ok(_repository.BuscarPorInmueble(entidad.InmuebleId));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}