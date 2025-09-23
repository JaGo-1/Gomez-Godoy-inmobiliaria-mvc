namespace inmobiliaria_mvc.ViewModels
{
    public class TablaViewModel<T> : PagedResult<T>
    {
        public List<string> Columnas { get; set; }
        public List<Dictionary<string, object>> Filas { get; set; }
    }

}