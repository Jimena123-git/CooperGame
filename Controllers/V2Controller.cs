using CooperGame.Data;
using CooperGame.Models;
using CooperGame.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static CooperGame.Models.Partida;

namespace CooperGame.Controllers
{
    public class V2Controller : Controller
    {
        private readonly AppDbContext _context;
        private readonly PartidaService _partidaService;

        public V2Controller(AppDbContext context, PartidaService partidaService)
        {
            _context = context;
            _partidaService = partidaService;
        }

        [HttpPost]
        public IActionResult CambiarNombreJugador(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Json(new { correcto = false, mensaje = "El nombre no puede estar vacío" });

            if (nombre.Length > 20 || !Regex.IsMatch(nombre, "^[a-zA-Z0-9]*$"))
                return Json(new { correcto = false, mensaje = "El nombre debe tener entre 1 y 20 caracteres y solo letras" });

            int? jugadorId = HttpContext.Session.GetInt32("JugadorId");
            Jugador jugador;

            if (jugadorId.HasValue)
            {
                jugador = _context.Jugadores.FirstOrDefault(j => j.IdJugador == jugadorId.Value)
                          ?? new Jugador(nombre);
                jugador.Nombre = nombre;

                if (jugador.IdJugador == 0)
                    _context.Jugadores.Add(jugador);
                else
                    _context.Jugadores.Update(jugador);
            }
            else
            {
                jugador = new Jugador(nombre);
                _context.Jugadores.Add(jugador);
            }

            _context.SaveChanges();
            HttpContext.Session.SetInt32("JugadorId", jugador.IdJugador);

            return Json(new { correcto = true, mensaje = $"Nombre actualizado a {nombre}", jugadorId = jugador.IdJugador });
        }

        [HttpGet("V2/AbrirMinijuego/{recurso}")]
        public IActionResult AbrirMinijuego(string recurso)
        {
            var numero = new Random();

            if (recurso == "Madera")
            {
                return PartialView("_MiniJuegoMadera", new
                {
                    num1 = numero.Next(1, 10),
                    num2 = numero.Next(1, 10),
                    num3 = numero.Next(1, 10)
                });
            }
            else if (recurso == "Piedra")
            {
                return PartialView("_MiniJuegoPiedra");
            }
            else if (recurso == "Comida")
            {
                return PartialView("_MiniJuegoComida", new
                {
                    numeros = new[] { numero.Next(1, 100), numero.Next(1, 100), numero.Next(1, 100) },
                    proposicion = new string[]
                    {
                        "Exactamente 2 números son pares",
                        "La suma de los 3 números es par",
                        "El número mayor es mayor que la suma de los otros dos",
                        "Hay al menos un número mayor que 50",
                        "Todos los números son diferentes"
                    }[numero.Next(5)]
                });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult ValidarMadera([FromBody] MaderaRequest request)
        {
            if (request == null) return JsonError("Datos inválidos");

            if (request.respuesta != request.num1 + request.num2 + request.num3)
                return JsonError($"❌ Respuesta incorrecta. La suma correcta era {request.num1 + request.num2 + request.num3}");

            var sesion = ObtenerSesion();
            if (sesion == null) return JsonError("Sesión inválida");

            RegistrarRecursoConMeta(sesion.Value.jugadorId, sesion.Value.partidaId, TipoRecurso.Madera);
            return JsonSuccess("✅ ¡Correcto! Has recolectado madera.");
        }

        [HttpPost]
        public IActionResult ValidarPiedra([FromBody] PiedraRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.pregunta) || string.IsNullOrEmpty(request.respuesta) || request.secuencia == null)
                return JsonError("Datos inválidos");

            var sesion = ObtenerSesion();
            if (sesion == null) return JsonError("Sesión inválida");

            bool correcta = request.respuesta == EvaluarPregunta(request.pregunta, request.secuencia);
            if (!correcta) return JsonError("❌ Respuesta incorrecta");

            RegistrarRecursoConMeta(sesion.Value.jugadorId, sesion.Value.partidaId, TipoRecurso.Piedra);
            return JsonSuccess("✅ ¡Correcto! Has recolectado piedra.");
        }

        [HttpPost]
        public IActionResult ValidarComida([FromBody] ComidaRequest request)
        {
            if (request == null || request.numeros == null || request.numeros.Length != 3 || string.IsNullOrEmpty(request.proposicion) || string.IsNullOrEmpty(request.respuesta))
                return JsonError("Datos inválidos");

            var sesion = ObtenerSesion();
            if (sesion == null) return JsonError("Sesión inválida");

            bool respuestaJugador = request.respuesta.ToLower() == "verdadero";
            bool esCorrecta = respuestaJugador == ComprobarCondicion(request.numeros, request.proposicion);

            if (!esCorrecta)
            {
                string respuestaCorrectaStr = ComprobarCondicion(request.numeros, request.proposicion) ? "Verdadero" : "Falso";
                return JsonError($"❌ Respuesta incorrecta. La respuesta correcta era: {respuestaCorrectaStr}");
            }

            RegistrarRecursoConMeta(sesion.Value.jugadorId, sesion.Value.partidaId, TipoRecurso.Comida);
            return JsonSuccess("✅ ¡Correcto! Has recolectado comida.");
        }

        private (int jugadorId, int partidaId)? ObtenerSesion()
        {
            int? jugadorId = HttpContext.Session.GetInt32("JugadorId");
            int? partidaId = HttpContext.Session.GetInt32("PartidaId");
            if (!jugadorId.HasValue || !partidaId.HasValue) return null;
            return (jugadorId.Value, partidaId.Value);
        }

        private void RegistrarRecursoConMeta(int jugadorId, int partidaId, TipoRecurso tipo)
        {
            var partida = _context.Partidas.FirstOrDefault(p => p.IdPartida == partidaId);
            if (partida == null) return;

            var registro = _context.Registros
                .FirstOrDefault(r => r.IdJugador == jugadorId && r.IdPartida == partidaId && r.Tipo == tipo);

            int meta = tipo switch
            {
                TipoRecurso.Comida => partida.MetaComida,
                TipoRecurso.Piedra => partida.MetaPiedra,
                TipoRecurso.Madera => partida.MetaMadera,
                _ => 0
            };

            int cantidadActual = registro?.Cantidad ?? 0;
            if (cantidadActual >= meta) return; // ya alcanzó la meta

            int cantidadAAgregar = 1; // siempre sumamos 1 por minijuego
            int nuevaCantidad = Math.Min(cantidadActual + cantidadAAgregar, meta);

            if (registro == null)
            {
                registro = new Registro
                {
                    IdJugador = jugadorId,
                    IdPartida = partidaId,
                    Tipo = tipo,
                    Cantidad = nuevaCantidad,
                    Fecha = DateTime.Now
                };
                _context.Registros.Add(registro);
            }
            else
            {
                registro.Cantidad = nuevaCantidad;
                _context.Registros.Update(registro);
            }

            // Verificar si todas las metas se completaron
            bool todasCompletadas =
                _context.Registros.Where(r => r.IdPartida == partidaId && r.Tipo == TipoRecurso.Comida).Sum(r => r.Cantidad) >= partida.MetaComida &&
                _context.Registros.Where(r => r.IdPartida == partidaId && r.Tipo == TipoRecurso.Piedra).Sum(r => r.Cantidad) >= partida.MetaPiedra &&
                _context.Registros.Where(r => r.IdPartida == partidaId && r.Tipo == TipoRecurso.Madera).Sum(r => r.Cantidad) >= partida.MetaMadera;

            if (todasCompletadas)
                partida.Estado = EstadoPartida.PresentandoResultados;

            _context.SaveChanges();
        }

        private string EvaluarPregunta(string pregunta, int[] numeros)
        {
            if (pregunta == "¿Había exactamente 2 números pares?")
                return numeros.Count(n => n % 2 == 0) == 2 ? "Sí" : "No";

            if (pregunta == "¿Había exactamente 2 números impares?")
                return numeros.Count(n => n % 2 != 0) == 2 ? "Sí" : "No";

            if (pregunta == "¿La suma de todos los números superaba 50?")
                return numeros.Sum() > 50 ? "Sí" : "No";

            if (pregunta == "¿Había 2 números iguales?")
                return numeros.GroupBy(n => n).Any(g => g.Count() > 1) ? "Sí" : "No";

            if (pregunta == "¿Había algún número menor a 10?")
                return numeros.Any(n => n < 10) ? "Sí" : "No";

            return "No";
        }

        private bool ComprobarCondicion(int[] numeros, string pregunta)
        {
            int mayor = Math.Max(numeros[0], Math.Max(numeros[1], numeros[2]));

            if (pregunta == "Exactamente 2 números son pares")
                return numeros.Count(n => n % 2 == 0) == 2;

            if (pregunta == "La suma de los 3 números es par")
                return (numeros.Sum() % 2 == 0);

            if (pregunta == "El número mayor es mayor que la suma de los otros dos")
                return mayor > (numeros.Sum() - mayor);

            if (pregunta == "Hay al menos un número mayor que 50")
                return numeros.Any(n => n > 50);

            if (pregunta == "Todos los números son diferentes")
                return numeros.Distinct().Count() == 3;

            return false;
        }

        private JsonResult JsonError(string mensaje) => Json(new { correcto = false, mensaje });
        private JsonResult JsonSuccess(string mensaje) => Json(new { correcto = true, mensaje });
    }

    // Modelos de request
    public record MaderaRequest(int num1, int num2, int num3, int respuesta);
    public record PiedraRequest(string pregunta, string respuesta, int[] secuencia);
    public record ComidaRequest(int[] numeros, string proposicion, string respuesta);
}
