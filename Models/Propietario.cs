using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_mvc.Models
{
    public class Propietario
    {
        public int Id { get; set; }
        [Required]
        public int Dni { get; set; }
        [Required]
        public required string Nombre { get; set; }
        [Required]
        public required string Apellido { get; set; }
        public required string Telefono { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required, MinLength(6)]
        public required string Clave { get; set; }

        public override string ToString()
        {
            return $"{Nombre} {Apellido}";
        }
    }
}