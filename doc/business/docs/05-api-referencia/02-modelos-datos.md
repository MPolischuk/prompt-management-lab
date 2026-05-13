---
sidebar_position: 2
title: Modelos de datos
---

# Modelos de datos

Los tipos expuestos al cliente TypeScript están en `src/PromptLab.ClientApp/src/types/index.ts` y reflejan el contrato JSON en **camelCase**.

## Prompts

- **`Prompt`**: `id`, `title`, `description`, `content`, `category`, `language`, `modelHint`, `targetModelId`, parámetros de generación (`temperature`, `maxTokens`, `topP`), `version`, `isActive`, fechas, `tags`, `tagSummaries`.
- **`PromptTagSummary`**: `id`, `name`, `slug`.
- **`PromptVersion`**: `id`, `promptId`, `content`, `version`, `createdAt`.

### Upsert (API .NET)

`UpsertPromptRequest` incluye `title`, `content`, `isActive`, colección **`tagIds`** (GUIDs) y campos opcionales de metadatos y generación.

## Tags

- **`Tag`**: `id`, `name`, `slug`, `createdAt`, `updatedAt`.

## Suites y casos

- **`TestSuite`**, **`TestCase`**, **`TestSuiteDetail`** (`suite` + `cases[]`).
- En API, `CreateTestCaseRequest` usa **`inputVariables`** como **string** (el front serializa un objeto a JSON).

## Runs y resultados

- **`TestRun`**: vínculo a `suiteId`, `promptId`, `promptVersion`, `model`, `temperature`, `status`, fechas de inicio/fin, nombres desnormalizados opcionales.
- **`TestResult`**: resultado por caso (`caseId`, `actualOutput`, `passed`, `score`, `latencyMs`, `error`, metadatos del caso).
- **`TestRunDetail`**: `run` + `results[]`.

## IA

- **`AiModel`**: `id`, `displayName`, `provider`, `enabled`.

## Paginación

- **`PagedResponse<T>`**: `items`, `pageNumber`, `pageSize`, `totalRows`.

## Análisis (entidades .NET)

`AnalyzeRequest` incluye `promptId`, `provider`, `modelId`, `input` opcional y `settings` (`GenerationSettings`) para la corrida.

## Siguiente

- [Errores y OperationResult](./03-errores)
