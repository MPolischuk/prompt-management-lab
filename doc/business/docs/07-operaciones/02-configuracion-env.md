---
sidebar_position: 2
title: Configuración de entorno
---

# Configuración de entorno

## Backend — `appsettings.json` / `appsettings.Development.json`

### `Sql:ConnectionString`

Cadena de SQL Server. En desarrollo típico:

```text
Server=.;Database=PromptLab.DB;Trusted_Connection=True;TrustServerCertificate=True;
```

### `Cache`

TTL en segundos para caché de tags, búsqueda de prompts y proveedores de catálogo.

### `Ai`

- **`DefaultProvider`**: slug del proveedor por defecto (`openai`, etc.).
- **`EnabledProviders` / `CatalogProviders`**: listas de proveedores.
- **`Providers:OpenAi|Anthropic|Google`**:
  - `Enabled`, `Mock`
  - `ApiKey`, `BaseUrl` (opcionales según entorno)
  - `TimeoutSeconds`
  - `Models`: `{ Id, DisplayName, Enabled }`
  - Anthropic: `AnthropicApiVersion`

En **Development** los proveedores suelen tener **`Mock: true`** para trabajar sin claves reales.

## Frontend — variables Vite

| Variable | Descripción |
|----------|-------------|
| `VITE_API_BASE_URL` | URL base del API (sin barra final). Si se omite, el cliente usa `https://localhost:7106`. |

Ejemplo (PowerShell):

```powershell
$env:VITE_API_BASE_URL="https://localhost:7106"; npm run dev
```

## Cabecera de versión

El cliente ya envía **`x-api-version: 1.0`**. Si el API cambia de versión mayor, actualizar cliente y documentación al unísono.

## Seguridad

- No commitear **API keys** reales; usar secretos de usuario, `dotnet user-secrets` o variables de entorno en CI/CD.

## Volver

- [Configuración backend](../03-backend/06-configuracion)
