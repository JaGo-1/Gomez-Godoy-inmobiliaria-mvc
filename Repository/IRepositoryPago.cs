using inmobiliaria_mvc.Models;

namespace inmobiliaria_mvc.Repository;

public interface IRepositoryPago : IRepository<Pago>
{
    int GenerarPrimerPagoParaContrato(int contratoId, decimal montoMensual, DateTime fechaInicio);
    int? CrearSiguientePagoSiAplica(int contratoId, int ultimoNumeroPago, decimal montoMensual, DateTime fechaInicio, DateTime fechaFin);
    int Modificacion(Pago pago, bool esRegistroReal = false);
    IList<Pago> ObtenerPorContrato(int contratoId, bool incluirAnulados = false);
    int AnularPago(int idPago);
}