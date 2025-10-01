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


        public Jugador(string nombre)
        {
            Nombre = nombre;

            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre no puede estar vacío o ser nulo.", nameof(nombre));


            }
        }

    }
}