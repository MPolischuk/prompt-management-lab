---
sidebar_position: 1
title: Gestión de prompts
---

# Gestión de prompts

Los **prompts** son plantillas de texto que pueden incluir **variables** en la forma `{{nombre}}` para ser sustituidas en tiempo de prueba o análisis.

## Crear y editar

- Desde el listado se accede a **nuevo prompt** o **editar** un existente.
- El formulario permite definir **contenido**, **nombre**, **tags** (selector con búsqueda y creación) y **modelo objetivo** (`ModelSelector` alimentado por `GET /api/ai-models`).
- Tras guardar, la aplicación navega al detalle del prompt.

## Versionado

- El detalle del prompt puede mostrar el **historial de versiones** (`GET /api/Prompts/{id}/versions`).
- Cada actualización del contenido relevante genera una nueva entrada de versión en backend (según la lógica de `PromptService` / repositorio).

## Tags

- Los tags permiten filtrar en el listado principal.
- La asociación prompt–tags se puede gestionar en creación/edición y mediante `PUT /api/Prompts/{id}/tags`.

## Enlaces técnicos

- [Páginas del frontend](../04-frontend/03-paginas)
- [Referencia de API](../05-api-referencia/01-endpoints)
