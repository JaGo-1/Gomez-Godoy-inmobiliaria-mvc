using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_mvc.Models;

public class Inquilino
{
    [Key]
    [Display(Name = "Código")]
    public int IdInquilino { get; set; }

    [Required]
    [StringLength(50)]
    public string Nombre { get; set; }

    [Required]
    [StringLength(50)]
    public string Apellido { get; set; }

    public string NombreCompleto => $"{Nombre} {Apellido}";


    [Required]
    public string Dni { get; set; }

    [Phone]
    public string? Telefono { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    public bool Estado { get; set; }
}

