---
sidebar_position: 3
title: Stack tecnológico
---

# Stack tecnológico

## Backend

| Tecnología | Uso |
|------------|-----|
| **.NET 10** | Runtime y SDK del servicio y bibliotecas. |
| **ASP.NET Core** | API REST, middleware, CORS. |
| **RepoDb.SqlServer** | Acceso a SQL Server sin Entity Framework. |
| **Microsoft.Data.SqlClient** | Conexión ADO.NET. |
| **Procedimientos almacenados** | Contrato de persistencia (`dbo.*` definidos en código como constantes). |

No hay **DbContext** ni **migraciones EF**; el esquema se gestiona en la base de datos (scripts / DBA).

## Frontend

| Tecnología | Uso |
|------------|-----|
| **React** | UI declarativa. |
| **Vite** | Bundler y servidor de desarrollo (puerto por defecto **5173**). |
| **TypeScript** | Tipado de componentes y cliente API. |
| **react-router-dom** | Enrutamiento del SPA. |
| **TanStack React Query** | Caché, estados de carga y revalidación de datos remotos. |
| **Tailwind CSS** | Estilos utilitarios. |

## Documentación (este sitio)

| Tecnología | Uso |
|------------|-----|
| **Docusaurus 3** | Sitio estático, MDX/Markdown, sidebar autogenerado. |
| **Node.js ≥ 20** | Requisito del motor de Docusaurus. |

## Volver

- [Visión general](./01-vision-general)
