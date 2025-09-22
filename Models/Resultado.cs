using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CooperGame.Models
{
    public class Resultado
    {
        [Key]
        public int IdResultado { get; set; }
        public int CantidadResultadoPorJugador { get; set; }
        public int IdJugador { get; set; }
        public Jugador? JugadorResultado { get; set; }
        public int IdPartida { get; set; }
        public Partida? Partida { get; set; }
    }
}
