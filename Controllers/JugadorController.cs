using CooperGame.Data;
using CooperGame.Models;
using CooperGame.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static CooperGame.Models.Partida;

namespace CooperGame.Controllers
{

    public class JugadorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PartidaService _partidaService;

        public JugadorController(AppDbContext context, PartidaService partidaService)
        {
            _context = context;
            _partidaService = partidaService;
        }

       
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Crear jugador y guardar en session
        [HttpPost]
        public IActionResult Registrar(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                TempData["Error"] = "El nombre no puede estar vacío.";
                return RedirectToAction("Index", "Home");
            }

            var jugador = _context.Jugadores.FirstOrDefault(j => j.Nombre == nombre);

            if (jugador == null)
            {
                jugador = new Jugador(nombre);
                _context.Jugadores.Add(jugador);
                _context.SaveChanges();
            }

            // Guardar jugador en sesión
            HttpContext.Session.SetInt32("JugadorId", jugador.IdJugador);
            HttpContext.Session.SetString("JugadorNombre", jugador.Nombre);

            // Revisar si el jugador tiene una partida activa
            var partida = _context.Partidas
                .Include(p => p.Registros)
                .FirstOrDefault(p => p.Registros.Any(r => r.IdJugador == jugador.IdJugador)
                                     && p.Estado == Partida.EstadoPartida.Jugando);

            // Si no tiene partida activa, crear una nueva con el servicio
            if (partida == null)
            {
                partida = _partidaService.CrearPartida();
            }

            // Guardar partida en sesión
            HttpContext.Session.SetInt32("PartidaId", partida.IdPartida);
            TempData["Mensaje"] = $"Bienvenido, {jugador.Nombre} 👋";
            return RedirectToAction("Index", "Home");
        }
    }
}
  