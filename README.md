# Prompt Management Lab

Laboratorio de gestión de prompts: API **ASP.NET Core** (.NET 10), base **SQL Server** (proyecto SSDT) y frontends en **React (Vite)** y **Next.js**.

## Estructura del repositorio

| Ruta | Descripción |
|------|-------------|
| `src/PromptLab.Service` | API web (`PromptLab.Service`), lógica (`PromptLab.Business`), datos (`PromptLab.Data`), modelos (`PromptLab.Entities`), integraciones IA (`PromptLab.Integrations`). |
| `src/PromptLab.DB` | Proyecto de base de datos SQL Server: tablas, funciones y stored procedures. |
| `src/PromptLab.ClientApp` | Cliente: React 18 + Vite + TypeScript + Tailwind + React Router + TanStack Query contra la API. Ver [README del ClientApp](src/PromptLab.ClientApp/README.md). |
| `src/tests/backend/PromptLab.Business.Tests` | Tests unitarios del backend. |

## Novedades recientes (resumen)

### Base de datos (`PromptLab.DB`)

- Columna **`Version`** en `Prompts` e historial en **`PromptVersions`** (cada cambio de contenido incrementa versión y registra fila en historial vía SPs `Prompt_Create` / `Prompt_Update`).
- **`Prompt_Search`** devuelve también `Content` y **`Prompt_GetById`** incluye tags en JSON (`TagsJson`) para el listado y el detalle.
- Módulo de pruebas: tablas **`TestSuites`**, **`TestCases`**, **`TestRuns`**, **`TestResults`** y SPs asociados (CRUD suites/casos, runs, resultados).

### API (`PromptLab.Service`)

- Endpoints existentes: prompts (CRUD + búsqueda paginada + tags), tags, análisis (`/api/Analyze`), catálogo de modelos y proveedores.
- Nuevos: **`GET /api/Prompts/{id}/versions`**, **`/api/TestSuites`**, **`/api/TestCases`**, **`/api/TestRuns`** (incluye `POST /api/TestRuns/{id}/results`).
- CORS habilitado para `http://localhost:3000` y **`http://localhost:5173`** (Vite).

### Frontend principal (`PromptLab.ClientApp`)

- Misma línea visual y flujos generales que la ReferenceApp, pero consumiendo **solo** la API .NET (sin Supabase).
- Variables de entorno: `VITE_API_BASE_URL` (por defecto en código suele alinearse con `https://localhost:7106`; ver `launchSettings.json` del servicio).

## Arquitectura backend (resumen)

- **PromptLab.Service**: controladores, versionado API (`x-api-version`), Swagger, composition root.
- **PromptLab.Business**: casos de uso, caché de búsqueda de prompts, orquestación de análisis con proveedores IA.
- **PromptLab.Data**: RepoDb + stored procedures.
- **PromptLab.Entities**: DTOs y contratos de repositorio.

## Requisitos

- .NET SDK 10
- Node.js 20+ (recomendado)
- SQL Server (local o remoto)

## Configuración de la API

Archivos típicos:

- `src/PromptLab.Service/PromptLab.Service/appsettings.json`
- `src/PromptLab.Service/PromptLab.Service/appsettings.Development.json`

Configurar al menos:

- `Sql:ConnectionString`
- `Ai:DefaultProvider`, `Ai:EnabledProviders`, `Ai:CatalogProviders`
- `Ai:Providers:*` (OpenAI, Anthropic, Google): `Enabled`, `Mock`, `ApiKey`, `BaseUrl` opcional, `TimeoutSeconds`

Variables de entorno para claves (no commitear valores reales):

- `Ai__Providers__OpenAi__ApiKey`
- `Ai__Providers__Anthropic__ApiKey`
- `Ai__Providers__Google__ApiKey`

Con `Mock: true` no hace falta `ApiKey` y las respuestas son simuladas.

## Ejecutar el backend

Desde la raíz del repo:

```powershell
dotnet build prompt-management-lab.sln
dotnet test prompt-management-lab.sln
dotnet run --project src/PromptLab.Service/PromptLab.Service/PromptLab.Service.csproj
```

Swagger: según perfil local, suele ser `https://localhost:7106/swagger` (revisar `Properties/launchSettings.json`).

## Publicar / actualizar la base de datos

Desplegar el proyecto **PromptLab.DB** contra tu instancia SQL Server (Visual Studio SSDT, `sqlpackage`, o pipeline de CI). 

## Ejecutar PromptLab.ClientApp 

```powershell
cd src/PromptLab.ClientApp
copy .env.example .env
# Editar .env: VITE_API_BASE_URL=https://localhost:7106 (o el puerto que uses)
npm install
npm run dev
```

Más detalle: [src/PromptLab.ClientApp/README.md](src/PromptLab.ClientApp/README.md).