using inmobiliaria_mvc.Models;

namespace inmobiliaria_mvc.ViewModels
{
    public class PropietarioDetalleVM
    {
        public Propietario Propietario { get; set; }
        public List<Inmueble> Inmuebles { get; set; } = new List<Inmueble>();
    }

}