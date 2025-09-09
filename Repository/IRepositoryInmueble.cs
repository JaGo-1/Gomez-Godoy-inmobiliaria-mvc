using inmobiliaria_mvc.Models;

namespace inmobiliaria_mvc.Repository;

public interface IRepositoryInmueble : IRepository<Inmueble>
{
    List<Inmueble> ObtenerPorPropietario(int propietarioId);
}