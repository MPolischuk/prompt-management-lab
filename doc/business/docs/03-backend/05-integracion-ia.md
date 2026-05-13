---
sidebar_position: 5
title: Integración IA
---

# Integración IA

La integración con proveedores externos se reparte entre **`PromptLab.Integrations`** (clientes HTTP) y **`PromptLab.Business`** (fábrica, opciones y orquestación).

## `AiProviderFactory`

- Expone los nombres de proveedores configurados.
- **`Resolve`**: obtiene la implementación `IAiProvider` según el nombre (OpenAI, Anthropic, Google, etc.).

## Proveedores (`IAiProvider`)

Cada proveedor implementa análisis asíncrono sobre un `Prompt` y un `AnalyzeExecutionRequest`.

## Clientes HTTP

Interfaces dedicadas (por ejemplo clientes GPT / Claude / Gemini) encapsulan:

- Construcción de URL y cabeceras (`Authorization`, `x-api-key`, claves en query según vendor).
- Parseo de respuestas JSON a texto de salida.

## Resolución de modelo

- **`AnalyzeModelResolver`**: decide qué combinación modelo + proveedor aplicar a partir del request y del estado del `Prompt` (por ejemplo `TargetModelId` o hints).
- **`AnalyzeCatalogService`** y **`AnalyzeModelCatalog`**: unen configuración estática y catálogo interno de modelos.

## Modo mock y claves

En `appsettings.Development.json` los proveedores pueden ejecutarse en **`Mock: true`** para desarrollo sin llamadas reales. En producción se configuran **API keys** y `Enabled` por proveedor.

## Siguiente

- [Configuración](./06-configuracion)
