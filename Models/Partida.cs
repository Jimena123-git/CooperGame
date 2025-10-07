using System.ComponentModel.DataAnnotations;

namespace CooperGame.Models
{
    public class Partida
    {
        [Key]
        public int IdPartida { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int MetaPiedra{  get; set; }

        public int MetaComida { get; set; }

        public int MetaMadera { get; set; }
        public List<Registro> Registros { get; set; }
                     
        public EstadoPartida Estado { get; set; } = EstadoPartida.Jugando;

        public enum EstadoPartida
        {
            Jugando,
            PresentandoResultados
        }
    }
}