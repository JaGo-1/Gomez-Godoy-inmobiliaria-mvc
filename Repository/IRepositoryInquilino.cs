using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.ViewModels;

namespace inmobiliaria_mvc.Repository;

public interface IRepositoryInquilino : IRepository<Inquilino>
{
    PagedResult<Inquilino> Paginar(int pagina, int tamPagina);

}