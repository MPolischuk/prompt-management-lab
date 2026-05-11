# PromptLab.ClientApp

Cliente React (Vite + TypeScript + Tailwind) contra **PromptLab.Service**.

## Configuración

1. Copiá `.env.example` a `.env` y ajustá `VITE_API_BASE_URL` (por defecto el servicio usa `https://localhost:7106` según `launchSettings.json`).
2. Arrancá la API .NET y publicá/actualizá la base **PromptLab.DB** (incluye tablas nuevas y SPs).
3. `npm install` y `npm run dev`.

## Cabeceras

El cliente envía `x-api-version: 1.0` y `Content-Type: application/json`.

## Rutas

- `/prompts` — listado y paginación
- `/prompts/new`, `/prompts/:id/edit` — formulario
- `/prompts/:id` — detalle e historial de versiones (`GET /api/Prompts/{id}/versions`)
- `/prompts/:promptId/test-suites` — suites
- `/test-suites/:suiteId` — casos y ejecución simulada
- `/test-runs`, `/test-runs/:runId` — análisis de runs
