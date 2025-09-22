using Microsoft.EntityFrameworkCore;
using CooperGame.Models;

namespace CooperGame.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Resultado> Resultados { get; set; }
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<Recurso> Recursos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Resultado>()
                .HasOne(resultado => resultado.JugadorResultado)
                .WithMany(jugador => jugador.Resultados)
                .HasForeignKey(resultado => resultado.IdJugador)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Resultado>()
                .HasOne(resultado => resultado.Partida)
                .WithMany(partida => partida.Resultados)
                .HasForeignKey(resultado => resultado.IdPartida)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partida>()
                .HasOne(partida => partida.JugadorPartida)
                .WithMany(jugador => jugador.Partidas)
                .HasForeignKey(partida => partida.IdJugador)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Recurso>()
                .HasOne(recursos => recursos.Partida)
                .WithMany(partida => partida.Recursos)
                .HasForeignKey(recursos => recursos.IdPartida)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }

}