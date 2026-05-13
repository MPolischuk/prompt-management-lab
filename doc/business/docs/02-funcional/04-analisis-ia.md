---
sidebar_position: 4
title: Análisis con IA
---

# Análisis con IA

El módulo de **análisis** permite ejecutar el contenido de un prompt contra un **proveedor** (OpenAI, Anthropic, Google) y un **modelo** concretos, respetando la configuración del sistema y del propio prompt.

## Proveedores y modelos

- Catálogo de proveedores: `GET /api/ai-providers`.
- Catálogo de modelos: `GET /api/ai-models` (combinación de configuración y catálogo interno).

## Ejecución

- `POST /api/Analyze` con un cuerpo `AnalyzeRequest` (referencia al prompt, ajustes de generación, etc.).
- El servicio de negocio **resuelve** modelo y proveedor (`AnalyzeModelResolver`), invoca el **proveedor** adecuado y **persiste** la corrida.
- Consulta de una corrida: `GET /api/Analyze/{id}`.

## Configuración

- Proveedores habilitados, mock, API keys y timeouts se definen en `appsettings` (sección `Ai`). Ver [Configuración de entorno](../07-operaciones/02-configuracion-env).

## Enlaces técnicos

- [Integración IA — backend](../03-backend/05-integracion-ia)
- [Referencia de API](../05-api-referencia/01-endpoints)
