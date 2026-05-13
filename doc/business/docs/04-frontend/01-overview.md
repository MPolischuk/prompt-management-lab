---
sidebar_position: 1
title: Overview del frontend
---

# Overview del frontend

El cliente vive en `src/PromptLab.ClientApp/` y es una **SPA** generada con **Vite**.

## Estructura principal

| Carpeta / archivo | Rol |
|-------------------|-----|
| `src/main.tsx` | Punto de entrada React. |
| `src/App.tsx` | Definición de rutas con `react-router-dom`. |
| `src/components/` | Layout (`AppLayout`), navegación (`Sidebar`), UI reutilizable (`Modal`, `Badge`, selectores). |
| `src/pages/` | Pantallas por dominio (prompts, suites, runs). |
| `src/lib/` | Cliente HTTP (`apiClient.ts`), funciones de dominio (`api.ts`), claves de React Query (`queryKeys.ts`). |
| `src/types/index.ts` | Tipos TypeScript alineados con la API en **camelCase**. |

## Datos remotos

Se usa **TanStack React Query** (`@tanstack/react-query`) con claves centralizadas en `queryKeys` para invalidar y reutilizar caché entre pantallas.

## Estilo

**Tailwind CSS** con tema oscuro en el layout principal (`bg-gray-950`).

## Siguiente

- [Rutas](./02-rutas)
