---
sidebar_position: 6
title: Configuración
---

# Configuración del backend

## `Sql`

| Clave | Descripción |
|-------|-------------|
| `ConnectionString` | Cadena SQL Server (ver `appsettings.json` y `appsettings.Development.json`). |

## `Cache`

TTL en segundos para:

- `TagsTtlSeconds`
- `PromptSearchTtlSeconds`
- `ProvidersTtlSeconds`

Los valores de **Development** suelen ser más bajos que los de plantilla base para facilitar pruebas.

## `Ai`

| Clave | Descripción |
|-------|-------------|
| `DefaultProvider` | Proveedor por defecto (p. ej. `openai`). |
| `EnabledProviders` | Lista de proveedores activos a nivel aplicación. |
| `CatalogProviders` | Proveedores considerados en el catálogo UI. |
| `Providers` | Sección anidada **OpenAi**, **Anthropic**, **Google** con `Enabled`, `Mock`, `ApiKey`, `BaseUrl`, `TimeoutSeconds`, `Models[]` (`Id`, `DisplayName`, `Enabled`). |

La validación en arranque garantiza que el proveedor por defecto esté habilitado y que los modelos no apunten a proveedores deshabilitados.

## Variables de entorno

Opcionalmente se pueden **sobreescribir** secciones mediante el sistema de configuración estándar de .NET (`ASPNETCORE_*`, variables con doble guión en CLI, etc.).

## Siguiente

- [Operaciones — configuración de entorno](../07-operaciones/02-configuracion-env)
