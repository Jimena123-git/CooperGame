using CooperGame.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Registro
{


    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdRegistro{ get; set; }

    public int IdJugador { get; set; }

   public int IdPartida { get; set; }
    public TipoRecurso Tipo { get; set; }
    public int Cantidad { get; set; }
    public DateTime Fecha { get; set; }
  
    public Jugador Jugador { get; set; }
    public Partida Partida { get; set; }
}