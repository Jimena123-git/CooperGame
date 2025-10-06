using System.ComponentModel.DataAnnotations;

namespace CooperGame.Models
{
    public class Partida
    {
        [Key]
        public int IdPartida { get; set; }

        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
    //   public List<Recurso>? Recursos { get; set; }
        public List<Registro>? Registros { get; set; }
                     
        public EstadoPartida Estado { get; set; } = EstadoPartida.Jugando;

        public enum EstadoPartida
        {
            Jugando,
            PresentandoResultados
        }
    }
}