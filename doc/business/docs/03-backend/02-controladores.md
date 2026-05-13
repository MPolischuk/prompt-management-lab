---
sidebar_position: 2
title: Controladores
---

# Controladores

Todos los controladores listados usan **`[ApiController]`**, **`[ApiVersion("1.0")]`** y rutas bajo `api/…`.

| Controlador | Ruta base | Métodos |
|-------------|-----------|---------|
| `PromptsController` | `api/Prompts` | `GET` búsqueda, `GET {id}`, `POST`, `PUT {id}`, `DELETE {id}`, `PUT {id}/tags` |
| `PromptVersionsController` | `api/Prompts` | `GET {promptId}/versions` |
| `TagsController` | `api/Tags` | `GET` (query opcional), `POST` |
| `TestSuitesController` | `api/TestSuites` | `GET` (query `promptId`), `GET {id}`, `POST`, `PUT {id}`, `DELETE {id}` |
| `TestCasesController` | `api/TestCases` | `GET` (query `suiteId`), `POST`, `PUT {id}`, `DELETE {id}` |
| `TestRunsController` | `api/TestRuns` | `GET` (query opcional `suiteId`), `GET {id}`, `POST`, `PUT {id}`, `POST {runId}/results` |
| `AnalyzeController` | `api/Analyze` | `POST` (cuerpo `AnalyzeRequest`), `GET {id}` |
| `AiModelsController` | `api/ai-models` | `GET` |
| `AiProvidersController` | `api/ai-providers` | `GET` |

### Utilidades

- **`OperationResultHttpMapper`**: extensión interna `ToHttpResult` para mapear `OperationResult` a respuestas HTTP coherentes.

## Notas

- Las respuestas `201 Created` incluyen cabecera `Location` con rutas en minúsculas según implementación actual (`/api/prompts/...`, `/api/testruns/...`, etc.).
- El cliente web debe enviar siempre **`Content-Type: application/json`** y la cabecera de versión acordada.

## Siguiente

- [Servicios Business](./03-servicios-business)
