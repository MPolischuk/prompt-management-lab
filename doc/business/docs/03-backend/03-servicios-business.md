---
sidebar_position: 3
title: Servicios Business
---

# Servicios Business

Los servicios viven en `PromptLab.Business` y exponen interfaces en `Services/Contracts`. Se registran en `ServiceCollectionExtensions.AddPromptLabBusiness`.

## `PromptService` (`IPromptService`)

- CRUD de prompts y **búsqueda paginada** con **caché en memoria** por versión lógica de la clave de búsqueda.
- **`SetTagsAsync`**: reasigna tags y **invalida** la caché de búsqueda cuando tiene éxito.
- **`GetVersionsAsync`**: delega en `IPromptVersionRepository`.

## `TagService` (`ITagService`)

- **`GetAllAsync`**: lista completa cacheada (TTL configurable).
- **`SearchAsync`**: si la query está vacía, reutiliza `GetAllAsync`; si no, consulta al repositorio.
- **`CreateAsync`**: invalida caché solo en éxito.

## `TestSuiteService`, `TestCaseService`, `TestRunService`

- Operaciones CRUD delegadas en repositorios.
- `TestSuiteService` compone **`TestSuiteDetail`** (suite + casos).
- `TestRunService` compone **`TestRunDetail`** (run + resultados).

## `AnalyzeService` (`IAnalyzeService`)

- **`AnalyzeAsync`**: carga el prompt, **resuelve** modelo y proveedor (`AnalyzeModelResolver`), ejecuta `IAiProvider` y persiste vía `IAnalyzeRepository`.
- **`GetProvidersAsync` / `GetModelsAsync`**: catálogo expuesto con **caché** y fusión con proveedores habilitados en configuración.
- **`GetByIdAsync`**: lectura de una corrida de análisis.

## Validación de opciones

- **`AiOptionsValidator`**: valida coherencia de `DefaultProvider`, `EnabledProviders` y modelos por proveedor (cubierto por tests).

## Siguiente

- [Capa de datos](./04-capa-datos)
