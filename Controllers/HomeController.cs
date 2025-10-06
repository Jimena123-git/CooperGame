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
        private readonly ILogger<HomeController> _logger;
        private readonly MetaServices _metaService;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, MetaServices metaService, AppDbContext context)
        {
            _logger = logger;
            _metaService = metaService;
            _context = context;
        }

        public IActionResult Index()
        {

            int jugadorId = TempData["JugadorId"] != null ? (int)TempData["JugadorId"] : 1;
            // Buscar la última partida en estado Jugando
            Partida partida = _context.Partidas.Include(p => p.Recursos).Include(p => p.Registros).ThenInclude(r => r.Jugador).Where(p => p.Estado == EstadoPartida.Jugando || p.Estado == EstadoPartida.PresentandoResultados)
            .OrderByDescending(p => p.FechaInicio).FirstOrDefault();

            // Buscar o crear jugador por Id
            Jugador jugador = _context.Jugadores.FirstOrDefault(j => j.IdJugador == jugadorId);
            if (jugador == null)
            {
                jugador = new Jugador("Jugador1");
                _context.Jugadores.Add(jugador);
                _context.SaveChanges();
                jugadorId = jugador.IdJugador;
            }

            // Crear partida si no hay ninguna
            if (partida == null)
            {
                partida = new Partida
                {
                    FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                    Estado = EstadoPartida.Jugando,
                    Recursos = new List<Recurso>()
                };

                _metaService.GenerarMetasV1(partida, 1.0);
                _context.Partidas.Add(partida);
                _context.SaveChanges(); ;
            }

            // Obtener registros de la partida
            List<Registro> registros = _context.Registros
            .Include(r => r.Jugador).Where(r => r.IdPartida == partida.IdPartida).ToList();

            // Revisar si todas las metas están completas
            bool todasCompletas = partida.Recursos.All(r =>
            registros.Where(reg => reg.Tipo == r.Tipo).Sum(reg => reg.Cantidad) >= r.Meta
            );

            if (todasCompletas && partida.Estado == EstadoPartida.Jugando)
            {
                partida.Estado = EstadoPartida.PresentandoResultados;
                _context.Partidas.Update(partida);
                _context.SaveChanges();
            }

            ViewData["JugadorNombre"] = jugador.Nombre;
            ViewData["JugadorId"] = jugador.IdJugador;
            ViewData["PartidaId"] = partida.IdPartida;
            ViewData["EstadoPartida"] = partida.Estado;
            ViewData["Registros"] = registros;
            TempData["JugadorId"] = jugador.IdJugador;

            return View(partida.Recursos);
        }

        [HttpPost]
        public IActionResult Recolectar(int idJugador, int idPartida, TipoRecurso tipo, int cantidad)
        {
            // Evitar crear Registro con FK no resuelta
            Jugador jugador = _context.Jugadores.FirstOrDefault(j => j.IdJugador == idJugador);
            Partida partida = _context.Partidas.FirstOrDefault(p => p.IdPartida == idPartida);
            if (jugador == null || partida == null)
                return RedirectToAction("Index");

            Registro registro = _context.Registros.FirstOrDefault(r => r.IdJugador == idJugador && r.IdPartida == idPartida && r.Tipo == tipo);

            if (registro != null)
            {
                registro.Cantidad += cantidad;
                registro.Puntaje += cantidad;
                _context.Registros.Update(registro);
            }
            else
            {
                registro = new Registro
                {
                    Jugador = jugador,
                    IdJugador = jugador.IdJugador,
                    IdPartida = partida.IdPartida,
                    Tipo = tipo,
                    Cantidad = cantidad,
                    Puntaje = cantidad,
                    Fecha = DateTime.Now
                };
                _context.Registros.Add(registro);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CambiarNombre(int idJugador, string nuevoNombre)
        {
            Jugador jugador = _context.Jugadores.FirstOrDefault(j => j.IdJugador == idJugador);
            if (jugador != null)
            {
                jugador.Nombre = nuevoNombre;
                _context.SaveChanges();
            }

            TempData["JugadorId"] = idJugador;
            return RedirectToAction("Index");
        }

        public IActionResult V2()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReiniciarPartida(string nuevoNombre)
        {
            // 1. Crear nuevo jugador cada vez
            Jugador jugador = new Jugador(string.IsNullOrWhiteSpace(nuevoNombre) ? "Jugador1" : nuevoNombre);
            _context.Jugadores.Add(jugador);
            _context.SaveChanges(); // se genera jugador.Id

            // 2. Crear nueva partida
            Partida nuevaPartida = new Partida
            {
                FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                Estado = EstadoPartida.Jugando,
                Recursos = new List<Recurso>()
            };

            _metaService.GenerarMetasV1(nuevaPartida, 1.0);

            _context.Partidas.Add(nuevaPartida);
            _context.SaveChanges(); // se genera partida.Id

            // 3. Guardar recursos (solo asignar IdPartida)
            foreach (Recurso recurso in nuevaPartida.Recursos)
            {
                recurso.IdPartida = nuevaPartida.IdPartida;
                _context.Recursos.Add(recurso);
            }
            _context.SaveChanges();

            // 4. Guardar IdJugador en TempData para la “sesión”
            TempData["JugadorId"] = jugador.IdJugador;

            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
