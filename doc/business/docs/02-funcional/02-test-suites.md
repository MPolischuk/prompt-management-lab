---
sidebar_position: 2
title: Test suites
---

# Test suites

Una **suite de pruebas** agrupa **casos** asociados a un **prompt** concreto. Sirve para validar el comportamiento del prompt ante distintas entradas (variables).

## Flujo en la aplicación

1. Desde el detalle de un prompt, se navega a **suites de prueba** (`/prompts/:promptId/test-suites`).
2. Se listan las suites del prompt; se pueden **crear** (nombre, descripción) o **eliminar**.
3. Al abrir una suite (`/test-suites/:suiteId`) se muestran los **casos** y las **ejecuciones** previas de esa suite.

## Casos de prueba

- Cada caso tiene **entrada esperada** (JSON de variables que alimentan el contenido del prompt).
- Se pueden **crear**, **editar** y **eliminar** casos desde el detalle de la suite.
- La UI detecta variables del prompt para orientar la edición.

## Datos en API

- Listado por prompt: `GET /api/TestSuites?promptId={guid}`.
- Detalle con casos: `GET /api/TestSuites/{id}`.

## Enlaces técnicos

- [Frontend — páginas](../04-frontend/03-paginas)
- [API — TestSuites y TestCases](../05-api-referencia/01-endpoints)
