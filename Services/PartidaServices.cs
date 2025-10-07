using CooperGame.Data;
using CooperGame.Models;
using Microsoft.EntityFrameworkCore;

namespace CooperGame.Services
{
    public class PartidaService
    {
        private readonly AppDbContext _context;
        private readonly MetaServices _metaService;

        public PartidaService(AppDbContext context, MetaServices metaService)
        {
            _context = context;
            _metaService = metaService;
        }
        public Partida CrearPartida()
        {
            // 1️⃣ Crear partida y guardar para que IdPartida exista
            Partida partida = new Partida
            {
                FechaInicio = DateTime.Now,
                Estado = Partida.EstadoPartida.Jugando,
                MetaComida = _metaService.GenerarMetasPorPartida(),
                MetaPiedra = _metaService.GenerarMetasPorPartida(),
                MetaMadera = _metaService.GenerarMetasPorPartida(),
                Registros= new List<Registro>()

            };

            Console.WriteLine($"Comida: {partida.MetaComida}, Piedra: {partida.MetaPiedra}, Madera: {partida.MetaMadera}");

            _context.Partidas.Add(partida);
            _context.SaveChanges(); // aquí ya se genera IdPartida


            return partida;
        }
    }
}
