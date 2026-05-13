---
sidebar_position: 3
title: Páginas
---

# Páginas

## `PromptsPage`

- Lista paginada de prompts (`searchPrompts`) con filtros por texto y **tag**.
- Carga tags globales (`getTags`) para el filtro.
- Acciones: ir a detalle, editar, suites de prueba, eliminar (`deletePrompt` + invalidación de `queryKeys.prompts`).

## `PromptFormPage`

- Modo **crear** (`/prompts/new`) o **editar** (`/prompts/:id/edit`) según presencia de `id` en la URL.
- Carga el prompt existente con `getPrompt` y permite editar campos alineados a `UpsertPromptBody`.
- Integra `TagSelector` y `ModelSelector`.
- Tras guardar, navega al detalle del prompt.

## `PromptDetailPage`

- Muestra metadatos, variables detectadas, tags y acciones (copiar contenido, eliminar).
- Opcionalmente muestra **versiones** (`getPromptVersions`).

## `TestSuitesPage`

- Contexto: `promptId` en la ruta.
- Lista suites del prompt; creación en modal; enlace al detalle de suite.

## `TestSuiteDetailPage`

- Muestra suite, casos y runs del suite.
- CRUD de casos (`createTestCase`, `updateTestCase`, `deleteTestCase`).
- Flujo **Ejecutar pruebas**: crea run, actualiza estado, envía resultados y navega al detalle del run.

## `TestRunsPage`

- Lista global de runs (`getAllTestRuns`) con filtros de estado y estadísticas agregadas.

## `TestRunDetailPage`

- Detalle de un run (`getTestRunDetail`): métricas y tabla de resultados.

## Siguiente

- [Componentes](./04-componentes)
