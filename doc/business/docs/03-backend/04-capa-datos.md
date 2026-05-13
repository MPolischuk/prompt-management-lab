---
sidebar_position: 4
title: Capa de datos
---

# Capa de datos (`PromptLab.Data`)

La persistencia usa **RepoDb** contra **SQL Server**. No hay Entity Framework ni migraciones en el repositorio de código.

## Arranque

`AddPromptLabData`:

- Configura `RepoDb` para SQL Server.
- Registra `IDbConnectionFactory` → `SqlConnectionFactory` con opciones `Sql:ConnectionString`.
- Registra repositorios concretos: prompts, versiones, tags, analyze, suites, casos, runs.

## Procedimientos almacenados

La clase **`StoredProcedures`** centraliza los nombres `dbo.*`, por ejemplo:

- `dbo.Prompt_*`, `dbo.PromptVersion_*`, `dbo.Tag_*`
- `dbo.TestSuite_*`, `dbo.TestCase_*`, `dbo.TestRun_*`, `dbo.TestResult_*`
- `dbo.Analyze_*`

Los repositorios invocan estos SPs y mapean filas a entidades de `PromptLab.Entities`.

## `PromptRepository` (ejemplo)

Implementa `IPromptRepository` con lógica de mapeo (incluye serialización JSON de tags en lecturas). Métodos típicos: `CreateAsync`, `UpdateAsync`, `DeleteAsync`, `GetByIdAsync`, `SearchAsync`, `SetTagsAsync`.

## Nota sobre el dominio de resultados de test

El código referencia el namespace **`PromptLab.Entities.TestResults`** (`CreateTestResultRequest`, etc.). Si el proyecto no compila, conviene **alinear** los archivos de entidades con ese namespace o ajustar los `using` en repositorio y servicio.

## Siguiente

- [Integración IA](./05-integracion-ia)
