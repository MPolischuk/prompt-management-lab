---
sidebar_position: 2
title: Rutas
---

# Rutas (`App.tsx`)

Todas las rutas están anidadas bajo **`AppLayout`** (sidebar + `Outlet`).

| Ruta | Componente | Parámetros |
|------|------------|------------|
| `/` | `Navigate` → `/prompts` | — |
| `/prompts` | `PromptsPage` | — |
| `/prompts/new` | `PromptFormPage` | — |
| `/prompts/:id` | `PromptDetailPage` | `id` |
| `/prompts/:id/edit` | `PromptFormPage` | `id` (edición) |
| `/prompts/:promptId/test-suites` | `TestSuitesPage` | `promptId` |
| `/test-suites/:suiteId` | `TestSuiteDetailPage` | `suiteId` |
| `/test-runs` | `TestRunsPage` | — |
| `/test-runs/:runId` | `TestRunDetailPage` | `runId` |

## Navegación lateral

`Sidebar` enlaza al menos **Prompts** (`/prompts`) y **Test runs** (`/test-runs`), resaltando la ruta activa con `useLocation()`.

## Siguiente

- [Páginas](./03-paginas)
