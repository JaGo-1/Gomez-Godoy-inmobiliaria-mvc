using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.ViewModels;

namespace inmobiliaria_mvc.Repository;

public interface IRepositoryInmueble : IRepository<Inmueble>
{
    List<Inmueble> ObtenerPorPropietario(int propietarioId);
    PagedResult<Inmueble> Paginar(int pagina, int tamPagina);
}