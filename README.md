# Huella Alimentaria – MVP (Supply Chain + Ciberseguridad)

> **"La seguridad que puedes saborear."**

Este proyecto implementa un **Sistema de Trazabilidad Cibersegura** para la **Cadena de Suministro Alimentaria**, abordando la necesidad crítica de **integridad de datos** y **transparencia** desde la granja hasta la mesa.

###  Problema de Supply Chain
La falta de transparencia y los registros alterables dificultan la gestión de retiradas de productos (recalls), el combate al fraude alimentario, y la optimización de los procesos logísticos.

###  Solución (Propuesta de Valor)
Se utiliza una estructura de **hash encadenado estilo Blockchain** para garantizar la **inmutabilidad** y la **auditabilidad** de cada evento (Origen, Procesamiento, Transporte, Distribución) de un lote de producto, permitiendo una consulta pública instantánea mediante códigos QR.

---

## Funcionalidades
- Productos, lotes y eventos de trazabilidad (Origen → Procesamiento → Transporte → Distribución).
- Integridad: cada evento incluye `prevHash` y `hash` (SHA‑256) sobre su contenido.
- Consulta pública por lote y generación de QR que apunta al endpoint del lote.
- Preparado para CI en GitHub Actions y despliegue con Docker.

## Ejecutar (desarrollo)
```bash
cd backend
dotnet restore
dotnet run
# Swagger en http://localhost:5000/swagger
```

## Endpoints
- `GET  /health` – chequeo de vida.
- `POST /api/products` – crea producto.
- `GET  /api/products/{id}` – obtiene producto.
- `POST /api/trace/batch` – crea lote.
- `GET  /api/trace/batch/{id}` – obtiene lote + validación de cadena.
- `POST /api/trace/event` – agrega evento encadenado.
- `GET  /api/trace/batch/{id}/qr` – PNG con QR del endpoint público del lote.

## Demo rápida (curl)
```bash
# Crear producto
PID=$(curl -s -X POST http://localhost:5000/api/products  -H "Content-Type: application/json"  -d '{"name":"Lechuga orgánica","category":"Vegetal"}' | jq -r '.id')

# Crear lote
BID=$(curl -s -X POST http://localhost:5000/api/trace/batch  -H "Content-Type: application/json"  -d "{"productId":"$PID","lotCode":"L-2025-0001"}" | jq -r '.id')

# Eventos
curl -s -X POST http://localhost:5000/api/trace/event  -H "Content-Type: application/json"  -d "{"batchId":"$BID","stage":"Origen","location":"Finca Los Pinos","data":"{\"cert\":\"GlobalG.A.P.\"}"}"

curl -s -X POST http://localhost:5000/api/trace/event  -H "Content-Type: application/json"  -d "{"batchId":"$BID","stage":"Transporte","location":"Ruta CR-32","data":"{\"temp\":\"4C\"}"}"

# Consultar
curl -s http://localhost:5000/api/trace/batch/$BID | jq

# Ver QR: http://localhost:5000/api/trace/batch/$BID/qr
```

## Ciberseguridad
- **Integridad** con hash encadenado.
- **Auditoría** completa por lote.
- **Confidencialidad**: añadir JWT/OAuth2 antes de exponer escritura en producción.
- **Cabeceras de seguridad** básicas incluidas; refuérzalas con un proxy reverso (Nginx) para HSTS, CSP, etc.

## Licencia
MIT
