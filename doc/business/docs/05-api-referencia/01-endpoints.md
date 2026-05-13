---
sidebar_position: 1
title: Endpoints
---

# Endpoints HTTP

Base típica en desarrollo: `https://localhost:7106`.

Todas las peticiones deben incluir la cabecera **`x-api-version: 1.0`** (o la versión acordada). El JSON usa **camelCase** en serialización por defecto de ASP.NET Core.

## Prompts (`/api/Prompts`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/Prompts` | Búsqueda paginada. Query: `query`, `category`, `language`, `isActive`, `tagId`, `createdFrom`, `createdTo`, `pageNumber`, `pageSize`. |
| `GET` | `/api/Prompts/{id}` | Detalle de un prompt. |
| `POST` | `/api/Prompts` | Crea prompt. Cuerpo: `UpsertPromptRequest`. Respuesta exitosa: `201` + `OperationResult` con `entityId`. |
| `PUT` | `/api/Prompts/{id}` | Actualiza prompt. Cuerpo: `UpsertPromptRequest`. |
| `DELETE` | `/api/Prompts/{id}` | Elimina prompt. |
| `PUT` | `/api/Prompts/{id}/tags` | Reemplaza tags: cuerpo **array de GUID** (`tagIds`). |

## Versiones de prompt (`/api/Prompts`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/Prompts/{promptId}/versions` | Lista de `PromptVersion` para un prompt. |

## Tags (`/api/Tags`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/Tags` | Lista completa o búsqueda si `?query=` está presente. |
| `POST` | `/api/Tags` | Crea tag. Cuerpo: `{ "name": "..." }`. |

## Test suites (`/api/TestSuites`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/TestSuites?promptId={guid}` | Suites de un prompt. |
| `GET` | `/api/TestSuites/{id}` | Detalle con casos (`TestSuiteDetail`). |
| `POST` | `/api/TestSuites` | Crea suite. Cuerpo: `CreateTestSuiteRequest`. |
| `PUT` | `/api/TestSuites/{id}` | Actualiza suite. Cuerpo: `UpdateTestSuiteRequest`. |
| `DELETE` | `/api/TestSuites/{id}` | Elimina suite. |

## Test cases (`/api/TestCases`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/TestCases?suiteId={guid}` | Casos de una suite. |
| `POST` | `/api/TestCases` | Crea caso. Cuerpo: `CreateTestCaseRequest` (`inputVariables` como string, p. ej. JSON). |
| `PUT` | `/api/TestCases/{id}` | Actualiza caso. Cuerpo: `UpdateTestCaseRequest`. |
| `DELETE` | `/api/TestCases/{id}` | Elimina caso. |

## Test runs (`/api/TestRuns`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/TestRuns` | Todos los runs, o filtro `?suiteId={guid}`. |
| `GET` | `/api/TestRuns/{id}` | Detalle con resultados (`TestRunDetail`). |
| `POST` | `/api/TestRuns` | Crea run. Cuerpo: `CreateTestRunRequest`. |
| `PUT` | `/api/TestRuns/{id}` | Actualiza estado/fechas. Cuerpo: `UpdateTestRunRequest`. |
| `POST` | `/api/TestRuns/{runId}/results` | Añade resultado. Cuerpo: `TestResultCreateBody` (`caseId`, `actualOutput`, `passed`, `score`, `latencyMs`, `error`). |

## Analyze (`/api/Analyze`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/Analyze` | Ejecuta análisis. Cuerpo: `AnalyzeRequest`. |
| `GET` | `/api/Analyze/{id}` | Obtiene una corrida persistida. |

## Catálogo IA

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/ai-models` | Modelos disponibles para UI / análisis. |
| `GET` | `/api/ai-providers` | Proveedores disponibles. |

## Siguiente

- [Modelos de datos](./02-modelos-datos)
