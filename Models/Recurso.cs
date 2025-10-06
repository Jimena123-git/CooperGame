using System.ComponentModel.DataAnnotations;

namespace CooperGame.Models
{
    public class Recurso
    {
        [Key]
        public int IdRecurso { get; set; }

       public TipoRecurso Tipo { get; set; }
    }

    public enum TipoRecurso
    {
        Madera,
        Piedra,
        Comida
    }
}