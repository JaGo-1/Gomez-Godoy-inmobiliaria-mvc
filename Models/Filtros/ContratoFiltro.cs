namespace inmobiliaria_mvc.Models.Filtros
{
    public class ContratoFiltro
    {
        public bool? Disponible { get; set; }
        public int? Plazo { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public string? Inquilino { get; set; }
        public string? Direccion { get; set; }
        public string? Termino  { get; set; }
        public ContratoFiltro() { }
    }
}
