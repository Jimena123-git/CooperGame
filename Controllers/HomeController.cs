using CooperGame.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using CooperGame.Services;
using CooperGame.Data;

namespace CooperGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly MetaServices _metaService;

        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, MetaServices metaService, AppDbContext context)
        {
            _logger = logger;
            _metaService=  metaService;
            _context = context;
        }

   
        public IActionResult Index()
        {
            Partida partida = new Partida();
            _metaService.GenerarMetasV1(partida, 1.0);
            //PRUEBA DE OUTPUT PARA VERIFICAR QUE LAS METAS FUERON HECHAS
            //
            foreach (var recurso in partida.Recursos)
            {
                Debug.WriteLine($"Tipo: {recurso.Tipo}, Meta: {recurso.Meta}");
            }

            return View();
        }

        public IActionResult V2()
        {
            return View();
        }
      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //NUEVA 25/09

        public IActionResult CrearPartida()
        {
            
            Jugador jugador = new Jugador("Ignacio");
            _context.Jugadores.Add(jugador);
            _context.SaveChanges(); 
            Partida partida = new Partida
            {
                IdJugador = jugador.IdJugador,
                JugadorPartida = jugador,
                FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                Recursos = new List<Recurso>()
            };
                       
            _metaService.GenerarMetasV1(partida, 1.0);
            _context.Partidas.Add(partida);
            _context.SaveChanges();

            return View(partida);
        }
    }
}