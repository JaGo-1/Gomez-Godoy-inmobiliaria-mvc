using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace inmobiliaria_mvc.Models;

public class Inmueble
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Dirección")]
    [Required(ErrorMessage = "La dirección es requerida")]
    public string Direccion { get; set; }

    [Required]
    public decimal Precio { get; set; }

    [Required]
    public int Ambientes { get; set; }

    [Required]
    public bool Estado { get; set; }

    [Required]
    public double Latitud { get; set; }
    [Required]
    public double Longitud { get; set; }

    [Required]
    public UsoInmueble Uso { get; set; }

    [Required]
    public TipoInmueble Tipo { get; set; }

    [Display(Name = "Dueño")]
    public int PropietarioId { get; set; }
    [ForeignKey(nameof(PropietarioId))]
    [BindNever]
    public Propietario? Propietario { get; set; }
}