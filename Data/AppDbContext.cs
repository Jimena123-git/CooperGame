using CooperGame.Models;
using Microsoft.EntityFrameworkCore;

namespace CooperGame.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<Recurso> Recursos { get; set; }
        public DbSet<Registro> Registros { get; set; }
       


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Registro>()
                .HasKey(r => new { r.IdJugador, r.IdPartida, r.Tipo });

            modelBuilder.Entity<Registro>()
                .HasOne(r => r.Jugador)
                .WithMany(j => j.Registros)
                .HasForeignKey(r => r.IdJugador)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Registro>()
                .HasOne(r => r.Partida)
                .WithMany(p => p.Registros)
                .HasForeignKey(r => r.IdPartida)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
