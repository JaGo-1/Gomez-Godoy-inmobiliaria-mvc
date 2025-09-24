using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.ViewModels;

namespace inmobiliaria_mvc.Repository
{
    public interface IRepositoryContrato : IRepository<Contrato>
    {
        bool ExisteSolapado(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? contratoId = null);
        List<Contrato> ObtenerContratosPorInmueble(int idInmueble);
        PagedResult<Contrato> Paginar(int pagina, int tamPagina);
        bool TerminarAnticipado(int contratoId, DateTime fechaTerminacion, bool pagarMultaAhora = false);
        int CalcularMesesContrato(DateTime inicio, DateTime fin);
        int CalcularMesesTranscurridos(Contrato contrato, DateTime fechaTerminacion);
        int CalcularMesesAdeudados(Contrato contrato, DateTime fechaTerminacion);
        int CalcularMultaMeses(Contrato contrato, DateTime fechaTerminacion);
        decimal CalcularMultaImporte(Contrato contrato, DateTime fechaTerminacion);
    }
}