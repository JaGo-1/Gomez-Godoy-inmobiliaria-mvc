using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.ViewModels;

namespace inmobiliaria_mvc.Repository
{
    public interface IRepositoryContrato : IRepository<Contrato>
    {
        bool ExisteSolapado(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? contratoId = null);
        List<Contrato> ObtenerContratosPorInmueble(int idInmueble);
        PagedResult<Contrato> Paginar(int pagina, int tamPagina, bool? disponible);
    }
}