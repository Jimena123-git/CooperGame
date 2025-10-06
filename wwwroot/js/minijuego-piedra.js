let secuenciaPiedra = [];
let preguntaTipo = "";
let countdownPiedraInterval;

function generarSecuenciaPiedra() {
    secuenciaPiedra = [];
    while (secuenciaPiedra.length < 5) {
        let n = Math.floor(Math.random() * 20) + 1;
        if (secuenciaPiedra[secuenciaPiedra.length - 1] !== n) {
            secuenciaPiedra.push(n);
        }
    }
}

function mostrarSecuenciaPiedra() {
    const display = document.getElementById("secuenciaDisplay");
    const preguntaEl = document.getElementById("preguntaPiedra");
    display.innerText = "";
    preguntaEl.innerText = "";
    let i = 0;

    function mostrarNumero() {
        if (i < secuenciaPiedra.length) {
            display.innerText = secuenciaPiedra[i];
            setTimeout(() => {
                display.innerText = "";
                i++;
                setTimeout(mostrarNumero, 500);
            }, 1000);
        } else {
            display.innerText = "Preparándose pregunta...";
            setTimeout(generarPreguntaPiedra, 500);
        }
    }
    mostrarNumero();
}

function generarPreguntaPiedra() {
    const opciones = [
        "¿Había exactamente 2 pares?",
        "¿Había exactamente 2 impares?",
        "¿La suma era > 50?",
        "¿Había 2 números iguales?",
        "¿Algún número < 10?"
    ];
    preguntaTipo = opciones[Math.floor(Math.random() * opciones.length)];
    document.getElementById("preguntaPiedra").innerText = preguntaTipo;
    document.getElementById("secuenciaDisplay").innerText = "";
}

function validarRespuestaPiedra(resp) {
    const modalEl = document.getElementById("minijuegoPiedra");
    const modalInstance = bootstrap.Modal.getInstance(modalEl);
    let correcta = false;

    switch (preguntaTipo) {
        case "¿Había exactamente 2 pares?":
            correcta = secuenciaPiedra.filter(n => n % 2 === 0).length === 2; break;
        case "¿Había exactamente 2 impares?":
            correcta = secuenciaPiedra.filter(n => n % 2 !== 0).length === 2; break;
        case "¿La suma era > 50?":
            correcta = secuenciaPiedra.reduce((a, b) => a + b, 0) > 50; break;
        case "¿Había 2 números iguales?":
            correcta = new Set(secuenciaPiedra).size < secuenciaPiedra.length; break;
        case "¿Algún número < 10?":
            correcta = secuenciaPiedra.some(n => n < 10); break;
    }

    const mensajeEl = document.getElementById("mensajePiedra");
    if ((resp === "Sí" && correcta) || (resp === "No" && !correcta)) {
        window.recursos.Piedra++;
        mensajeEl.innerText = "✅ ¡Correcto! Ganaste 1 piedra";
        mensajeEl.className = "text-success mt-2";
        mensajeEl.style.display = "block";
        setTimeout(() => modalInstance.hide(), 1000);
    } else {
        mensajeEl.innerText = `❌ Incorrecto. Secuencia: [${secuenciaPiedra.join(", ")}]`;
        mensajeEl.className = "text-warning mt-2";
        mensajeEl.style.display = "block";
        setTimeout(() => modalInstance.hide(), 2000);
    }

    clearInterval(countdownPiedraInterval);
}

function abrirModalPiedra() {
    clearInterval(countdownPiedraInterval);
    generarSecuenciaPiedra();
    const modalEl = document.getElementById("minijuegoPiedra");
    const modal = new bootstrap.Modal(modalEl, { backdrop: 'static', keyboard: false });
    modal.show();

    let seg = 60;
    const countdown = document.getElementById("countdownPiedra");
    countdown.innerText = `Tiempo restante: ${seg}s`;

    countdownPiedraInterval = setInterval(() => {
        seg--;
        countdown.innerText = `Tiempo restante: ${seg}s`;
        if (seg <= 0) {
            clearInterval(countdownPiedraInterval);
            document.getElementById("mensajePiedra").innerText = "⏳ Tiempo agotado";
            document.getElementById("mensajePiedra").style.display = "block";
            modal.hide();
        }
    }, 1000);

    mostrarSecuenciaPiedra();
}

// Eventos
document.querySelector("button[data-recurso='Piedra']").addEventListener("click", abrirModalPiedra);
document.getElementById("btnSi").addEventListener("click", () => validarRespuestaPiedra("Sí"));
document.getElementById("btnNo").addEventListener("click", () => validarRespuestaPiedra("No"));
