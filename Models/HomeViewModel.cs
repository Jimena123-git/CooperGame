using CooperGame.Models;
using System.Collections.Generic;
using static CooperGame.Models.Partida;

public class HomeViewModel
{
    public int JugadorId { get; set; }
    public string JugadorNombre { get; set; }
    public  Partida? Partida{ get; set; }
    public Registro? Registro{ get; set; }   
    public EstadoPartida EstadoPartida { get; set; }
    public List<Registro> Registros { get; set; } = new List<Registro>();
}
