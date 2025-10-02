using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.ViewModels;

namespace inmobiliaria_mvc.Repository
{
    public interface IRepositoryAuditoria : IRepository<Auditoria>
    {
        PagedResult<Auditoria> Paginar(int pagina, int tamPagina);
    }
}