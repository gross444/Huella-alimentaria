# Seguridad
Este proyecto es un MVP educativo. Antes de exponer en producción:
- Habilitar **autenticación y autorización** (JWT, OAuth2).
- Agregar **CSP**, **HSTS**, **rate limiting** y **WAF** en el perímetro.
- Mover secretos a **variables de entorno**/secretos del proveedor cloud.
- Sustituir DB en memoria por **base de datos real** (con backups y cifrado en reposo).
- Auditorías periódicas y pruebas de seguridad (SAST/DAST).

## Reporte de vulnerabilidades
Abra un issue con etiqueta `security` o contacte a los mantenedores.
