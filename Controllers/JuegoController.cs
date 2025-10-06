using CooperGame.Data;
using CooperGame.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CooperGame.Services;
using Microsoft.Win32;

namespace CooperGame.Controllers
{
    public class JuegoController : Controller
    {
        private readonly AppDbContext _context;

        public JuegoController(AppDbContext context)
        {
            _context = context;
        }

        public class MinijuegosController : Controller
        {
            private readonly AppDbContext _context;

            public MinijuegosController(AppDbContext context)
            {
                _context = context;
            }

            // Vista principal con el tablero/juegos
            public IActionResult Index(int idPartida)
            {
                var partida = _context.Partidas
                    .Include(p => p.Resultados)
                    .Include(p => p.Recursos)
                    .FirstOrDefault(p => p.IdPartida == idPartida);

                if (partida == null) return NotFound();

                return View(partida);
            }

            // Guardar resultado de un minijuego
            [HttpPost]
            public IActionResult GuardarResultado([FromBody] Resultado resultado)
            {
                if (resultado == null) return BadRequest();

                _context.Resultados.Add(resultado);
                _context.SaveChanges();

                return Ok(new { mensaje = "Resultado guardado correctamente" });
            }
        }
    }
}
