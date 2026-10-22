# Huella Alimentaria – MVP de trazabilidad alimentaria

> **"Nuestra huella alimentaria: la seguridad que puedes saborear."**

Sistema de trazabilidad para la cadena de suministro alimentaria. Registra cada etapa de un lote de producto (Origen, Procesamiento, Transporte, Distribución) en una cadena de hashes SHA-256 que permite **detectar si algún registro fue alterado**, y ofrece una consulta pública mediante código QR.

## Problema

La falta de transparencia y los registros fáciles de alterar dificultan la gestión de retiradas de producto (recalls), el combate al fraude alimentario y la demostración de cumplimiento ante clientes y autoridades.

## Solución

Cada evento de un lote guarda el hash del evento anterior (`prevHash`) y el suyo propio (`hash`). Si alguien modifica un evento pasado, su hash deja de coincidir y la validación de la cadena falla. Cualquier persona puede consultar el historial de un lote y ver si la cadena es íntegra.

> **Nota sobre el alcance:** la cadena vive en un solo servidor y una sola base de datos. Detecta alteraciones, pero no es una blockchain descentralizada: quien controla el servidor podría reconstruir la cadena completa. Para mayor garantía, el hash final puede anclarse periódicamente en una red pública (ver próximos pasos).

## Funcionalidades

- Productos, lotes y eventos de trazabilidad (Origen → Procesamiento → Transporte → Distribución).
- Integridad con `prevHash` y `hash` (SHA-256) en cada evento.
- Validación de la cadena al consultar un lote.
- Generación de código QR por lote (PNG).
- Página pública de consulta del historial de un lote.
- Documentación interactiva de la API con Swagger.

## Requisitos

- [SDK de .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git (para clonar el repositorio)

## Ejecutar en local

```bash
git clone https://github.com/gross444/Huella-Alimentaria.git
cd Huella-Alimentaria/backend
dotnet restore
dotnet run
```

| Qué | Dirección |
|---|---|
| Página de consulta pública | http://localhost:5000 |
| Swagger (documentación y pruebas de la API) | http://localhost:5000/swagger |
| Chequeo de vida | http://localhost:5000/health |

## Probar el flujo completo

1. Abre Swagger y ejecuta `POST /api/products` con:
   ```json
   { "name": "Lechuga orgánica", "category": "Vegetal" }
   ```
   Copia el `id` del producto.
2. `POST /api/trace/batch` con ese id:
   ```json
   { "productId": "ID-DEL-PRODUCTO", "lotCode": "L-2025-0001" }
   ```
   Copia el `id` del **lote**.
3. `POST /api/trace/event`, una vez por etapa:
   ```json
   { "batchId": "ID-DEL-LOTE", "stage": "Origen", "location": "Finca Los Pinos", "data": "{\"cert\":\"GlobalG.A.P.\"}" }
   ```
   ```json
   { "batchId": "ID-DEL-LOTE", "stage": "Transporte", "location": "Ruta CR-32", "data": "{\"temp\":\"4C\"}" }
   ```
4. Abre http://localhost:5000, pega el id del lote y pulsa **Consultar Lote**. Verás los eventos y que el `prevHash` de cada uno coincide con el `hash` del anterior.
5. `GET /api/trace/batch/{id}/qr` devuelve el QR del lote.

> Por ahora los datos se guardan en memoria: se pierden al reiniciar la aplicación.

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/health` | Chequeo de vida |
| POST | `/api/products` | Crea un producto |
| GET | `/api/products/{id}` | Obtiene un producto |
| POST | `/api/trace/batch` | Crea un lote |
| GET | `/api/trace/batch/{id}` | Obtiene el lote, sus eventos y la validación de la cadena |
| POST | `/api/trace/event` | Agrega un evento encadenado |
| GET | `/api/trace/batch/{id}/qr` | PNG con el QR del lote |

## Docker (en pruebas)

```bash
docker compose up --build
```

Publica la API en http://localhost:5000. El contenedor de PostgreSQL ya está definido en `docker-compose.yml`, pero la API todavía no lo utiliza.

## Seguridad

- **Integridad:** hash encadenado SHA-256 para detectar alteraciones.
- **Auditoría:** historial completo y verificable por lote.
- **Pendiente antes de producción:** autenticación (JWT/OAuth2) en los endpoints de escritura, HTTPS, cabeceras de seguridad (HSTS, CSP) mediante un proxy inverso, y mover las credenciales de ejemplo (`mypassword`) a variables de entorno.

## Próximos pasos

- Persistencia en PostgreSQL con Entity Framework Core.
- QR que abra directamente la página de consulta del lote.
- Autenticación para registrar eventos.
- Anclaje periódico del hash final en una red pública.
- Pruebas automáticas e integración continua con GitHub Actions.

## Licencia

MIT
