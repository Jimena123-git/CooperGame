using System.ComponentModel.DataAnnotations;

namespace CooperGame.Models
{
    public class Recurso
    {
        [Key]
        public int IdRecurso { get; set; }
        public string? Objetivo { get; set; }
        public int CantidadRecolectada { get; set; }
        public int IdPartida { get; set; }
        public Partida? Partida { get; set; }
    }

    public enum TipoRecurso
    {
        Madera,
        Piedra,
        Comida
    }
}
