---
sidebar_position: 4
title: Componentes
---

# Componentes

## `AppLayout`

Contenedor de página completa: `Sidebar` + `Outlet` de React Router.

## `Sidebar`

Enlaces de primera línea a **Prompts** y **Test runs**; resalta la ruta actual.

## `Badge`

Indicadores visuales con variantes: `default`, `success`, `error`, `warning`, `info`, `neutral`.

## `Modal`

Diálogo accesible: cierre con Escape, clic en overlay y prop `size` (`sm` … `xl`).

## `Pagination`

Controles de paginación basados en `pageNumber`, `pageSize`, `totalRows` y callback `onPageChange`. Se oculta si solo hay una página.

## `TagSelector`

- Props: `selectedIds`, `onChange`.
- Lista y busca tags (`getTags`); permite crear tag nuevo (`createTag`).

## `ModelSelector`

- Props: `value`, `onChange` (id de modelo).
- Carga modelos con `getAiModels` y `queryKeys.aiModels`.

## Volver

- [Cliente API](./05-cliente-api)
