using inmobiliaria_mvc.Models;
using System.Collections.Generic;

namespace inmobiliaria_mvc.ViewModels
{
    public class InquilinoDetalleVM
    {
        public Inquilino? Inquilino { get; set; }
        public IList<Contrato> Contratos { get; set; } = new List<Contrato>();
        public IList<Pago> Pagos { get; set; } = new List<Pago>();
    }
}