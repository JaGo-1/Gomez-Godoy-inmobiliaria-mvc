using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliaria_mvc.Models
{
    public class Auditoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Entidad { get; set; }

        [Required]
        public int EntidadId { get; set; }

        [Required]
        [StringLength(50)]
        public string Accion { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        [Column(TypeName = "jsonb")]
        public string? DatosAnteriores { get; set; }

        [Column(TypeName = "jsonb")]
        public string? DatosNuevos { get; set; }
    }
}
