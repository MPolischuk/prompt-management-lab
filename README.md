# Prompt Management Lab

Repositorio de laboratorio para gestion de prompts con arquitectura por capas en .NET y frontend en Next.js.

## Estructura del repositorio

- `src/PromptLab.Service`: backend principal (`PromptLab.Service`, `PromptLab.Business`, `PromptLab.Data`, `PromptLab.Entities`).
- `src/tests/backend/PromptLab.Business.Tests`: tests unitarios del backend.
- `src/PromptLab.App`: frontend Next.js.
- `src/PromptLab.DB`: proyecto de base de datos SQL Server.

## Arquitectura backend (resumen)

- `PromptLab.Service`: API ASP.NET Core (controladores, versionado, Swagger, composition root).
- `PromptLab.Business`: logica de negocio y contratos de repositorio.
- `PromptLab.Data`: implementaciones de repositorio y acceso SQL (RepoDb + stored procedures).
- `PromptLab.Entities`: DTOs y modelos compartidos.

## Requisitos

- .NET SDK 10
- Node.js 20+ (recomendado)
- SQL Server (local o remoto)

## Configuracion

La API usa:

- `src/PromptLab.Service/PromptLab.Service/appsettings.json`
- `src/PromptLab.Service/PromptLab.Service/appsettings.Development.json`

Configurar al menos:

- `Sql:ConnectionString`
- `Ai:DefaultProvider`
- `Ai:EnabledProviders` (proveedores realmente implementados)
- `Ai:CatalogProviders` (catalogo visible de proveedores)
- `Ai:Providers:*` (OpenAI, Anthropic, Google): `Enabled`, `Mock`, `ApiKey` (secretos via variables de entorno o user-secrets), `BaseUrl` opcional, `TimeoutSeconds`

Ejemplos de variables de entorno para claves (sin commitear valores reales):

- `Ai__Providers__OpenAi__ApiKey`
- `Ai__Providers__Anthropic__ApiKey`
- `Ai__Providers__Google__ApiKey`

Con `Mock: true` no se requiere `ApiKey` y las respuestas son simuladas conservando el nombre logico del proveedor (`openai`, `anthropic`, `google`).

## Ejecutar backend

Desde la raiz del repo:

```powershell
dotnet build prompt-management-lab.sln
dotnet run --project src/PromptLab.Service/PromptLab.Service/PromptLab.Service.csproj
```

Swagger suele quedar disponible en `https://localhost:7106/swagger` (segun profile local).

## Ejecutar frontend

```powershell
cd src/PromptLab.App
npm install
npm run dev
```

El frontend usa `NEXT_PUBLIC_API_BASE_URL` y por defecto apunta a `https://localhost:7106/api`.

