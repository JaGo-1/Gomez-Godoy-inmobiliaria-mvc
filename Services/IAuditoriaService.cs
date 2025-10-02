namespace inmobiliaria_mvc.Services
{
    public interface IAuditoriaService
    {
        void RegistrarCambio(
            string entidad,
            int entidadId,
            string accion,
            int usuarioId,
            object datosAnteriores,
            object datosNuevos
        );
    }
}
