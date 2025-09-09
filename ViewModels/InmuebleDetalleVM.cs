using inmobiliaria_mvc.Models;

namespace inmobiliaria_mvc.ViewModels
{
    public class InmuebleDetalleVM
    {
        public Inmueble Inmueble { get; set; }
        public List<Contrato> Contratos { get; set; } = new List<Contrato>();
    }
}