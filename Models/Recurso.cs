using System.ComponentModel.DataAnnotations;

namespace CooperGame.Models
{
    public class Recurso
    {
        [Key]
        public int IdRecurso { get; set; }
        public int Meta { get; set; }
        public int CantidadRecolectada { get; set; }
        public int IdPartida { get; set; }
        public Partida? Partida { get; set; }

        public TipoRecurso Tipo { get; set; }
    }

    public enum TipoRecurso
    {
        Madera,
        Piedra,
        Comida
    }
}
