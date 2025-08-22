using inmobiliaria_mvc.Models;

namespace inmobiliaria_mvc.Repository
{
    public interface IRepositoryPropietario : IRepository<Propietario>
    {
        //IList<Propietario> BuscarPorNombre(string nombre);
        // Propietario BuscarPorEmail(string email);
        // IList<Propietario> ObtenerPorPagina(int pagina, int cantidad);
    }
}