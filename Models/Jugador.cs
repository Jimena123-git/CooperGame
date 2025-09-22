using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CooperGame.Models
{
    public class Jugador
    {
        [Key]
        public int IdJugador { get; set; }
        public string? Nombre { get; set; }
        public DateOnly FechaRegistro { get; set; }
        public List<Resultado>? Resultados { get; set; }
        public List<Partida>? Partidas { get; set; }

    }
}
