# PromptLab.ClientApp

Cliente **React 18** con **Vite**, **TypeScript**, **Tailwind CSS**, **React Router** y **TanStack Query**, consumiendo exclusivamente **PromptLab.Service** (sin Supabase).

Documentación general del repo: [README en la raíz](../../README.md).

## Requisitos

- Node.js 20+ (recomendado)
- API .NET en ejecución y base **PromptLab.DB** desplegada con el esquema actual (incluye tablas de versiones y de tests).

## Configuración

1. Copiá `.env.example` a `.env`.
2. Ajustá **`VITE_API_BASE_URL`** al origen HTTPS (o HTTP) de la API, **sin** barra final. El valor por defecto en `apiClient.ts` suele coincidir con `https://localhost:7106` según `PromptLab.Service/Properties/launchSettings.json`.
3. `npm install` y `npm run dev` (puerto típico de Vite: **5173**, ya permitido en CORS del backend).

## Cabeceras HTTP

Todas las peticiones envían:

- `Content-Type: application/json`
- `x-api-version: 1.0` (versionado de la API)

## Rutas de la SPA

| Ruta | Descripción |
|------|-------------|
| `/prompts` | Listado de prompts con búsqueda, filtro por tag (API) y paginación. |
| `/prompts/new` | Alta de prompt (título, contenido, modelo del catálogo, temperatura, tags por GUID). |
| `/prompts/:id` | Detalle, variables `{{...}}`, historial de versiones (`GET /api/Prompts/:id/versions`). |
| `/prompts/:id/edit` | Edición; al cambiar el contenido el servidor versiona y registra en `PromptVersions`. |
| `/prompts/:promptId/test-suites` | Suites de tests asociadas a ese prompt. |
| `/test-suites/:suiteId` | Casos de la suite, alta/edición/baja y ejecución simulada. |
| `/test-runs` | Listado global de ejecuciones con KPIs y filtro por estado. |
| `/test-runs/:runId` | Detalle de una run con resultados por caso. |

## Test suites y test runs (cómo encaja todo)

1. **Prompt**  
   El texto del prompt puede incluir placeholders `{{nombreVariable}}` (solo identificadores tipo palabra, p. ej. `{{metodo}}`, `{{ruta}}`).

2. **Test suite**  
   Agrupa pruebas sobre **un** prompt. No define las variables: solo organiza casos.

3. **Test case**  
   - **Nombre**: etiqueta para la UI.  
   - **Variables de entrada**: objeto JSON guardado en BD; las claves deben coincidir con los nombres dentro de `{{...}}` del prompt (sin llaves). Ejemplo: si el prompt menciona `{{metodo}}` y `{{ruta}}`, un caso puede tener `{"metodo":"GET","ruta":"/api/users"}`.  
   - **Salida esperada** (opcional): texto usado por la **simulación** del cliente para un chequeo muy simple (subcadena / heurística), no es un motor de assertions completo.

   En la pantalla de detalle de la suite, el modal detecta las variables leyendo el contenido del prompt y muestra un campo por cada una.

4. **Test run**  
   Una ejecución de la suite en un instante dado: estado, modelo, versión del prompt usada, etc.  
   **Ejecución actual**: simulada en el navegador: crea el run en la API, sustituye `{{variables}}` en el contenido del prompt con los valores del caso, genera una salida ficticia, persiste cada **test result** y marca la run como completada. No invoca `POST /api/Analyze` por cada caso (eso sería una evolución futura).

## Scripts

| Comando | Descripción |
|---------|-------------|
| `npm run dev` | Servidor de desarrollo Vite. |
| `npm run build` | `tsc -b` + build de producción. |
| `npm run preview` | Sirve el build generado. |

## Build de producción

```powershell
npm run build
```

Los artefactos quedan en `dist/`. Configurá el mismo `VITE_API_BASE_URL` en el entorno de build que usarán los usuarios contra la API desplegada.
