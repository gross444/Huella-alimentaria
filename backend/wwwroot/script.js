// Misma dirección que la API (el frontend se sirve desde wwwroot del backend)
const API_BASE = "";

const $ = (id) => document.getElementById(id);

function mostrarError(texto) {
    const msg = $("mensaje-error");
    msg.textContent = texto;
    msg.style.display = "block";
}

function fila(etiqueta, valor) {
    const p = document.createElement("p");
    const b = document.createElement("strong");
    b.textContent = etiqueta + ": ";
    p.appendChild(b);
    p.appendChild(document.createTextNode(valor));
    return p;
}

// Usamos textContent (nunca innerHTML) porque los datos de los eventos los escriben usuarios.
function crearEvento(ev) {
    const div = document.createElement("div");
    div.className = "evento";

    div.appendChild(fila("Etapa", ev.stage ?? "-"));
    div.appendChild(fila("Ubicación", ev.location ?? "-"));
    if (ev.timestamp) {
        div.appendChild(fila("Fecha", new Date(ev.timestamp).toLocaleString()));
    }

    if (ev.data) {
        let texto = ev.data;
        try {
            const obj = JSON.parse(ev.data);
            texto = Object.entries(obj).map(([k, v]) => `${k}: ${v}`).join(", ");
        } catch { /* si no es JSON, se muestra tal cual */ }
        div.appendChild(fila("Datos", texto));
    }

    const hashes = document.createElement("div");
    hashes.className = "hashes";
    const prev = document.createElement("small");
    prev.textContent = "prevHash: " + (ev.prevHash || "(inicio de la cadena)");
    const actual = document.createElement("small");
    actual.textContent = "hash: " + (ev.hash ?? "-");
    hashes.append(prev, actual);
    div.appendChild(hashes);

    return div;
}

function mostrarResultado(data) {
    const eventos = data.traceEvents ?? data.batch?.traceEvents ?? data.events ?? data.eventos ?? [];
    const valida = data.isChainValid ?? data.chainValid ?? data.isValid ?? data.valid ?? data.chain?.valid;
    const status = $("validacion-status");

    if (valida === true) {
        status.className = "status-ok";
        status.textContent = "Cadena íntegra: ningún evento ha sido alterado.";
    } else if (valida === false) {
        status.className = "status-error";
        status.textContent = "ALERTA: la cadena de hashes no es válida. Hay datos alterados.";
    } else {
        status.className = "";
        status.textContent = "Validación de cadena no disponible en la respuesta.";
        console.log("Respuesta de la API:", data);
    }

    const historial = $("historial-eventos");
    if (eventos.length === 0) {
        historial.textContent = "Este lote aún no tiene eventos registrados.";
        return;
    }
    eventos.forEach((ev) => historial.appendChild(crearEvento(ev)));
}

async function consultarTrazabilidad() {
    const id = $("batchIdInput").value.trim();

    $("resultado").style.display = "block";
    $("mensaje-error").style.display = "none";
    $("validacion-status").className = "";
    $("validacion-status").textContent = "";
    $("historial-eventos").replaceChildren();

    if (!id) {
        mostrarError("Ingresa un ID de lote.");
        return;
    }

    try {
        const res = await fetch(`${API_BASE}/api/trace/batch/${encodeURIComponent(id)}`);
        if (res.status === 404) {
            mostrarError("Lote no encontrado.");
            return;
        }
        if (!res.ok) throw new Error("código " + res.status);
        mostrarResultado(await res.json());
    } catch (e) {
        mostrarError("No se pudo consultar la API (" + e.message + ").");
    }
}

// Permite abrir la página con ?id=XXXX (útil para QR que apunten a la página)
window.addEventListener("DOMContentLoaded", () => {
    const id = new URLSearchParams(location.search).get("id");
    if (id) {
        $("batchIdInput").value = id;
        consultarTrazabilidad();
    }
});
