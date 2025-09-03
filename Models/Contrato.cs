using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace inmobiliaria_mvc.Models;

public class Contrato
{
    [Key]
    public int Id { get; set; }
    [Display(Name = "Inmuele")]
    public int IdInmueble { get; set; }
    [Display(Name = "Inquilino")]
    public int IdInquilino { get; set; }

    [ValidateNever]
    public Inquilino Inquilino { get; set; }
    [ValidateNever]
    public Inmueble Inmueble { get; set; }
    public int Monto { get; set; }
    [Display(Name = "Fecha de inicio")]
    public DateTime Fecha_inicio { get; set; }
    [Display(Name = "Fecha de fin")]
    public DateTime Fecha_fin { get; set; }

    [ValidateNever]
    public bool Estado { get; set; }
}