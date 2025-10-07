using CooperGame.Data;
using CooperGame.Models;
using CooperGame.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static CooperGame.Models.Partida;

namespace CooperGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PartidaService _partidaService;

        public HomeController(AppDbContext context, PartidaService partidaService)
        {
            _context = context;
            _partidaService = partidaService;
        }

        public IActionResult Index()
        {
            int? jugadorId = HttpContext.Session.GetInt32("JugadorId");
            string jugadorNombre = HttpContext.Session.GetString("JugadorNombre") ?? "Invitado";

            Partida? partida = _context.Partidas.Include(p => p.Registros)
                    .Where(p => p.Estado == EstadoPartida.Jugando)
                    .FirstOrDefault();

            

            if (partida == null)
            {
                partida = _context.Partidas.Include(p => p.Registros)
                          .Where(p => p.Estado == EstadoPartida.PresentandoResultados)
                          .FirstOrDefault();
            }
            
            if (partida == null)
            {
                partida = _partidaService.CrearPartida();
                HttpContext.Session.SetInt32("PartidaId", partida.IdPartida);
            }
            

            var vm = new HomeViewModel
            {
                JugadorId = jugadorId ?? 0,
                JugadorNombre = jugadorNombre,
                Partida = partida,
                EstadoPartida = partida?.Estado ?? EstadoPartida.Jugando,
                Registros = _context.Registros
                    .Where(r => r.IdPartida == partida.IdPartida)
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Recolectar(TipoRecurso tipo, int cantidad = 1)
        {
            int? jugadorId = HttpContext.Session.GetInt32("JugadorId");
            int? partidaId = HttpContext.Session.GetInt32("PartidaId");

            if (!jugadorId.HasValue || !partidaId.HasValue)
                return RedirectToAction("Index");

            Jugador? jugador = _context.Jugadores.FirstOrDefault(j => j.IdJugador == jugadorId);
            Partida? partida = _context.Partidas.FirstOrDefault(p => p.IdPartida == partidaId);


            if (jugador == null || partida == null)
                return RedirectToAction("Index");

            Registro? registro = _context.Registros
                .FirstOrDefault(r => r.IdJugador == jugador.IdJugador && 
                                     r.IdPartida == partida.IdPartida && 
                                     r.Tipo == tipo);

            int meta = tipo switch
            {
                TipoRecurso.Comida => partida.MetaComida,
                TipoRecurso.Piedra => partida.MetaPiedra,
                TipoRecurso.Madera => partida.MetaMadera,
                _ => 0
            };

            int cantidadActual = registro?.Cantidad ?? 0;
            int cantidadRestante = Math.Max(0, meta - cantidadActual);
            int cantidadARecolectar = Math.Min(cantidad, cantidadRestante);

            if (registro == null)
            {
                registro = new Registro
                {
                    IdJugador = jugador.IdJugador,
                    IdPartida = partida.IdPartida,
                    Tipo = tipo,
                    Cantidad = cantidadARecolectar,
                    Fecha = DateTime.Now
                };
                _context.Registros.Add(registro);
            }
            else
            {
                registro.Cantidad += cantidadARecolectar;
            }
            bool completadasTodas =
             partida.Registros.Where(r => r.Tipo == TipoRecurso.Comida).Sum(r => r.Cantidad) >= partida.MetaComida &&
             partida.Registros.Where(r => r.Tipo == TipoRecurso.Piedra).Sum(r => r.Cantidad) >= partida.MetaPiedra &&
             partida.Registros.Where(r => r.Tipo == TipoRecurso.Madera).Sum(r => r.Cantidad) >= partida.MetaMadera;

            if (completadasTodas)
            {
                partida.Estado = EstadoPartida.PresentandoResultados;
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            });
        }
    }
}
