using System.ComponentModel.DataAnnotations;

namespace CooperGame.Models
{
    public class Jugador
    {
        [Key]
        public int IdJugador { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 1 y 20 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "El nombre solo puede contener letras y números.")]
        public string? Nombre { get; set; }

        public DateOnly FechaRegistro { get; set; }
        public List<Registro> Registros { get; set; }
        public Jugador(string nombre)
        {
            Nombre = nombre;

            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre no puede estar vacío o ser nulo.", nameof(nombre));
            }
        }
    }
}