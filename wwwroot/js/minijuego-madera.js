// Recursos globales
window.recursos = { Madera: 0, Piedra: 0, Comida: 0 };

let minijuegoMaderaData = {};
let countdownMaderaInterval;

function generarMinijuegoMadera() {
    const modalPregunta = document.getElementById("modalPregunta");
    minijuegoMaderaData.a = Math.floor(Math.random() * 100) + 1;
    minijuegoMaderaData.b = Math.floor(Math.random() * 100) + 1;
    minijuegoMaderaData.c = Math.floor(Math.random() * 100) + 1;
    modalPregunta.innerText = `${minijuegoMaderaData.a} + ${minijuegoMaderaData.b} + ${minijuegoMaderaData.c} = ?`;
}

function abrirModalMadera() {
    clearInterval(countdownMaderaInterval);
    generarMinijuegoMadera();
    const modalEl = document.getElementById("minijuegoMadera");
    const modal = new bootstrap.Modal(modalEl, { backdrop: 'static', keyboard: false });
    modal.show();

    let seg = 60;
    const countdown = document.getElementById("countdownMadera");
    countdown.innerText = `Tiempo restante: ${seg}s`;

    countdownMaderaInterval = setInterval(() => {
        seg--;
        countdown.innerText = `Tiempo restante: ${seg}s`;
        if (seg <= 0) {
            clearInterval(countdownMaderaInterval);
            alert("Tiempo agotado");
            modal.hide();
        }
    }, 1000);
}

function enviarRespuestaMadera() {
    const valor = Number(document.getElementById("respuestaInput").value.trim());
    const suma = minijuegoMaderaData.a + minijuegoMaderaData.b + minijuegoMaderaData.c;
    const modalEl = document.getElementById("minijuegoMadera");
    const modalInstance = bootstrap.Modal.getInstance(modalEl);
    const modalMensaje = document.getElementById("modalMensaje");

    if (valor === suma) {
        window.recursos.Madera++;
        modalMensaje.innerText = "✅ ¡Correcto!";
        modalMensaje.className = "text-success mt-2";
        modalMensaje.style.display = "block";
        setTimeout(() => modalInstance.hide(), 1000);
    } else {
        modalMensaje.innerText = `❌ Incorrecto → La correcta era ${suma}`;
        modalMensaje.className = "text-warning mt-2";
        modalMensaje.style.display = "block";
        setTimeout(() => modalInstance.hide(), 2000);
    }
    clearInterval(countdownMaderaInterval);
}

document.querySelector("button[data-recurso='Madera']").addEventListener("click", abrirModalMadera);
document.getElementById("enviarRespuestaBtn").addEventListener("click", enviarRespuestaMadera);
document.getElementById("btnCancelarMadera").addEventListener("click", () => {
    clearInterval(countdownMaderaInterval);
    bootstrap.Modal.getInstance(document.getElementById("minijuegoMadera")).hide();
});
