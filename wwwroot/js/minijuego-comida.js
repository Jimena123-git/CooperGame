let numerosComida = [], proposicion = "", esVerdadera = false, countdownInterval;

function generarMinijuegoComida() {
    numerosComida = Array.from({ length: 3 }, () => Math.floor(Math.random() * 100) + 1);
    const proposiciones = [
        "Exactamente 2 pares",
        "Suma de los 3 números es par",
        "Número mayor > suma de los otros dos",
        "Al menos un número >50",
        "Todos los números diferentes"
    ];
    proposicion = proposiciones[Math.floor(Math.random() * proposiciones.length)];

    if (Math.random() < 0.3) {
        esVerdadera = true;
        ajustarNumerosVerdadera(proposicion);
    } else {
        esVerdadera = false;
        ajustarNumerosFalsa(proposicion);
    }

    document.getElementById("proposicionDisplay").innerText = proposicion;
    document.getElementById("numerosComidaDisplay")?.remove();

    const modalBody = document.querySelector("#minijuegoComida .modal-body");
    const p = document.createElement("p");
    p.id = "numerosComidaDisplay";
    p.className = "fw-bold mb-3";
    p.innerText = `Números: ${numerosComida.join(", ")}`;
    modalBody.insertBefore(p, modalBody.querySelector("#proposicionDisplay").nextSibling);
}

function ajustarNumerosVerdadera(prop) {
    let [a, b, c] = numerosComida;
    switch (prop) {
        case "Exactamente 2 pares": a = 2; b = 4; c = 5; break;
        case "Suma de los 3 números es par": a = 2; b = 4; c = 6; break;
        case "Número mayor > suma de los otros dos": a = 10; b = 3; c = 4; break;
        case "Al menos un número >50": a = 60; b = 20; c = 30; break;
        case "Todos los números diferentes": a = 1; b = 2; c = 3; break;
    }
    numerosComida = [a, b, c];
}

function ajustarNumerosFalsa(prop) {
    let [a, b, c] = numerosComida;
    switch (prop) {
        case "Exactamente 2 pares": a = 2; b = 2; c = 4; break;
        case "Suma de los 3 números es par": a = 1; b = 2; c = 3; break;
        case "Número mayor > suma de los otros dos": a = 3; b = 5; c = 10; break;
        case "Al menos un número >50": a = 10; b = 20; c = 30; break;
        case "Todos los números diferentes": a = 2; b = 2; c = 3; break;
    }
    numerosComida = [a, b, c];
}

function abrirModalComida() {
    clearInterval(countdownInterval);
    generarMinijuegoComida();

    const modalEl = document.getElementById("minijuegoComida");
    const modal = new bootstrap.Modal(modalEl, { backdrop: 'static', keyboard: false });
    modal.show();

    let seg = 60;
    const countdown = document.getElementById("countdownComida");
    countdown.innerText = `Tiempo restante: ${seg}s`;

    countdownInterval = setInterval(() => {
        seg--;
        countdown.innerText = `Tiempo restante: ${seg}s`;
        if (seg <= 0) {
            clearInterval(countdownInterval);
            mostrarMensajeComida("⏳ Tiempo agotado", false);
            modal.hide();
        }
    }, 1000);
}

function validarRespuestaComida(resp) {
    const modalEl = document.getElementById("minijuegoComida");
    const modalInstance = bootstrap.Modal.getInstance(modalEl);
    let correcta = (resp === "Verdadero" && esVerdadera) || (resp === "Falso" && !esVerdadera);

    if (correcta) {
        window.recursos.Comida++;
        mostrarMensajeComida("✅ ¡Correcto! Ganaste 1 comida", true);
        setTimeout(() => modalInstance.hide(), 1000);
    } else {
        mostrarMensajeComida(`❌ Incorrecto. Números: [${numerosComida.join(", ")}]. Explicación: ${proposicion} → ${esVerdadera ? "Verdadero" : "Falso"}`, false);
        setTimeout(() => modalInstance.hide(), 2000);
    }
    clearInterval(countdownInterval);
}

function mostrarMensajeComida(msg, ok) {
    const mensaje = document.getElementById("mensajeComida");
    mensaje.innerText = msg;
    mensaje.style.display = "block";
    mensaje.className = ok ? "text-success" : "text-warning";
}

document.querySelector("button[data-recurso='Comida']").addEventListener("click", abrirModalComida);
document.getElementById("btnVerdadero").addEventListener("click", () => validarRespuestaComida("Verdadero"));
document.getElementById("btnFalso").addEventListener("click", () => validarRespuestaComida("Falso"));
document.getElementById("btnCancelarComida").addEventListener("click", () => {
    clearInterval(countdownInterval);
    bootstrap.Modal.getInstance(document.getElementById("minijuegoComida")).hide();
});
