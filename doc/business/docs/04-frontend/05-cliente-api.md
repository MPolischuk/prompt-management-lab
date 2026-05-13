---
sidebar_position: 5
title: Cliente API
---

# Cliente API

## `apiClient.ts`

- **`getApiBaseUrl`**: lee `import.meta.env.VITE_API_BASE_URL` o usa por defecto `https://localhost:7106` (perfil HTTPS del backend en `launchSettings.json`).
- **Cabeceras por defecto**: `Content-Type: application/json` y **`x-api-version: 1.0`**.
- **`ApiError`**: error enriquecido con `status` HTTP y cuerpo `OperationResult` cuando el servidor lo devuelve.
- Helpers **`apiGet`**, **`apiPost`**, **`apiPut`**, **`apiDelete`** + **`assertSuccess`** para interpretar `OperationResult` en mutaciones.

## `api.ts`

Funciones de dominio que componen rutas y tipos:

| Área | Funciones |
|------|-----------|
| Prompts | `searchPrompts`, `getPrompt`, `getPromptVersions`, `createPrompt`, `updatePrompt`, `deletePrompt`, `setPromptTags` |
| Tags | `getTags`, `createTag` |
| IA | `getAiModels` |
| Suites | `getTestSuitesByPrompt`, `getTestSuiteDetail`, `createTestSuite`, `updateTestSuite`, `deleteTestSuite` |
| Casos | `getTestCasesBySuite`, `createTestCase`, `updateTestCase`, `deleteTestCase` |
| Runs | `getAllTestRuns`, `getTestRunsBySuite`, `getTestRunDetail`, `createTestRun`, `updateTestRun`, `createTestResult` |

Los cuerpos de **casos** serializan `inputVariables` como **JSON string** para coincidir con el contrato del backend.

## `queryKeys.ts`

Objeto **`queryKeys`** con claves estables (`as const`) para prompts, versiones, tags, modelos, suites, casos y runs — usadas en `useQuery` / `invalidateQueries`.

## Siguiente

- [Referencia API](../05-api-referencia/01-endpoints)
