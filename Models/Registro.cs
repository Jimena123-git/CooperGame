using CooperGame.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Registro
{
    public int Id { get; set; }

    [ForeignKey("Jugador")]
    public int IdJugador { get; set; }

    [ForeignKey("Partida")]
    public int IdPartida { get; set; }

    public TipoRecurso Tipo { get; set; }
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    public int Puntaje { get; set; }

    public Jugador Jugador { get; set; }
    public Partida Partida { get; set; }
}