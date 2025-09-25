using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_mvc.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        [Required]
        public int ContratoId { get; set; }
        [Required]
        public int NumeroPago { get; set; }
        [Required]
        public DateTime FechaEsperada { get; set; }
        public DateTime? FechaPago { get; set; }
        [Required]
        public decimal Importe { get; set; }
        public string? Detalle { get; set; }
        public bool Estado { get; set; }
        public Contrato? Contrato { get; set; }
        
        public bool EsMulta  { get; set; }
    }
}