using inmobiliaria_mvc.Models;

namespace inmobiliaria_mvc.Repository;

public interface IRepositoryImagen : IRepository<Imagen>
{
    IList<Imagen> BuscarPorInmueble(int inmuebleId);
}