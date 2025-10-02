using System.Text.Json;
using inmobiliaria_mvc.Models;
using inmobiliaria_mvc.Repository;

namespace inmobiliaria_mvc.Services
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IRepositoryAuditoria _repo;

        public AuditoriaService(IRepositoryAuditoria repo)
        {
            _repo = repo;
        }

        public void RegistrarCambio(string entidad, int entidadId, string accion, int usuarioId, object datosAnteriores, object datosNuevos)
        {
            string datosAnterioresJson = datosAnteriores != null ? JsonSerializer.Serialize(datosAnteriores) : null;
            string datosNuevosJson = datosNuevos != null ? JsonSerializer.Serialize(datosNuevos) : null;

            var auditoria = new Auditoria
            {
                Entidad = entidad,
                EntidadId = entidadId,
                Accion = accion,
                UsuarioId = usuarioId,
                DatosAnteriores = datosAnterioresJson,
                DatosNuevos = datosNuevosJson
            };

            _repo.Alta(auditoria);
        }
    }

}