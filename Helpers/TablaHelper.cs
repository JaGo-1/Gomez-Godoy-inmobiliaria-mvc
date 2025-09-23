using inmobiliaria_mvc.ViewModels;

namespace inmobiliaria_mvc.Helpers
{
    /// Convierte una lista paginada de cualquier entidad
    /// (Contrato, Inmueble, Inquilino, etc.) en un objeto genérico
    /// de tipo <see cref="TablaViewModel{T}"/> que sirve para renderizar
    /// tablas con columnas dinámicas y soporte de paginación.

    public static class TablaHelper
    {
        public static TablaViewModel<T> MapToTablaViewModel<T>(
            PagedResult<T> datos,
            Func<T, Dictionary<string, object>> mapFunc)
        {
            var filas = datos.Items.Select(mapFunc).ToList();

            return new TablaViewModel<T>
            {
                Columnas = filas.FirstOrDefault()?.Keys.ToList() ?? new List<string>(),
                Filas = filas,
                PageNumber = datos.PageNumber,
                TotalItems = datos.TotalItems,
                PageSize = datos.PageSize
            };
        }
    }
}