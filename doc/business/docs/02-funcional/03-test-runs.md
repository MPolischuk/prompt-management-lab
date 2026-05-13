---
sidebar_position: 3
title: Test runs
---

# Test runs

Un **run** representa una **ejecución** de una suite (o su seguimiento agregado en el listado global). Contiene **resultados por caso**: salida real, si pasó, puntuación, latencia y mensaje de error opcional.

## Ejecutar desde la suite

En el detalle de suite, la acción de **ejecutar pruebas**:

1. Crea un run vía API (`POST /api/TestRuns`).
2. Para cada caso, envía un resultado con `POST /api/TestRuns/{runId}/results` (cuerpo con `caseId`, `actualOutput`, `passed`, `score`, `latencyMs`, `error`).
3. En el prototipo actual, parte de la salida puede **simularse** en cliente con delays; la persistencia es real contra la API.

## Consulta

- Listado global: `/test-runs` — `GET /api/TestRuns` (opcionalmente filtrable por `suiteId` en backend).
- Detalle: `/test-runs/:runId` — `GET /api/TestRuns/{id}` con resultados anidados.

## Métricas en UI

- Tasa de éxito, latencia agregada, score y listado de resultados con badges de estado.

## Enlaces técnicos

- [Referencia de API — Test runs](../05-api-referencia/01-endpoints)
