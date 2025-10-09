using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace inmobiliaria_mvc.Models;

public enum EstadoContrato
{
    Anulado = 0,
    Vigente = 1,
    Finalizado = 2,
    Rescindido = 3
}

public class Contrato
{
    [Key] public int Id { get; set; }
    [Display(Name = "Inmueble")] public int IdInmueble { get; set; }
    [Display(Name = "Inquilino")] public int IdInquilino { get; set; }

    [ValidateNever] public Inquilino Inquilino { get; set; }
    [ValidateNever] public Inmueble Inmueble { get; set; }
    public decimal Monto { get; set; }
    [Display(Name = "Fecha de inicio")] public DateTime Fecha_inicio { get; set; }
    [Display(Name = "Fecha de fin")] public DateTime Fecha_fin { get; set; }
    public DateTime? FechaTerminacionAnticipada { get; set; }
    public decimal? MultaCalculada { get; set; }

    [ValidateNever] public EstadoContrato Estado { get; set; } = EstadoContrato.Vigente;
    public List<Pago> Pagos { get; set; } = new List<Pago>();
    
    [ValidateNever]
    public EstadoContrato EstadoEfectivo
    {
        get
        {
            if (this.Estado == EstadoContrato.Anulado) return EstadoContrato.Anulado;
            if (this.Estado == EstadoContrato.Rescindido) return EstadoContrato.Rescindido;

            if (this.Fecha_fin < DateTime.Today) return EstadoContrato.Finalizado;

            return this.Estado;
        }
    }
}