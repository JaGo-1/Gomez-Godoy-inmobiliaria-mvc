using inmobiliaria_mvc.Models;

namespace inmobiliaria_mvc.Repository
{
    public interface IRepositoryContrato : IRepository<Contrato>
    {
        bool ExisteSolapado(int idInmueble, DateTime fechaInicio, DateTime fechaFin);
    }
}