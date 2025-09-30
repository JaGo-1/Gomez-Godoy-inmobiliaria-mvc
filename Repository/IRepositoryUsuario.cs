using inmobiliaria_mvc.Models;

namespace inmobiliaria_mvc.Repository;

public interface IRepositoryUsuario : IRepository<Usuario>
{
    Usuario ObtenerPorEmail(string email);
}