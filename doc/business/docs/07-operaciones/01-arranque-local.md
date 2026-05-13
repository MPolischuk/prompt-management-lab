---
sidebar_position: 1
title: Arranque local
---

# Arranque local

## Prerrequisitos

- **.NET SDK 10** (alineado con `TargetFramework` `net10.0`).
- **Node.js ≥ 20** (motor de Docusaurus y Vite).
- **SQL Server** accesible con la cadena configurada en `appsettings.Development.json`.
- Herramienta de paquetes: **npm** o **yarn** (el `README` del sitio de docs usa `yarn`).

## Base de datos

1. Crear la base (nombre coherente con la cadena de conexión, p. ej. `PromptLab.DB` en desarrollo).
2. Aplicar los **scripts de procedimientos almacenados** y esquema requeridos por `StoredProcedures` / repositorios (no hay migraciones EF en el código).

## API (`PromptLab.Service`)

```bash
cd src/PromptLab.Service/PromptLab.Service
dotnet run --launch-profile https
```

URLs por defecto (`launchSettings.json`):

- HTTPS: `https://localhost:7106`
- HTTP: `http://localhost:5106`

Swagger UI queda habilitado en el mismo host bajo las rutas estándar de Swashbuckle.

## Cliente web (`PromptLab.ClientApp`)

```bash
cd src/PromptLab.ClientApp
npm install
npm run dev
```

Por defecto Vite sirve en **`http://localhost:5173`**, origen ya permitido por CORS en `Program.cs`.

## Documentación Docusaurus (`doc/business`)

```bash
cd doc/business
yarn install
yarn start
```

El sitio de documentación se sirve en el puerto que indique la consola (típicamente `3000`).

## Orden recomendado

1. SQL Server arriba y base creada.
2. API (`dotnet run`).
3. Cliente (`npm run dev`) con `VITE_API_BASE_URL` apuntando al API si no usas el default.

## Siguiente

- [Configuración de entorno](./02-configuracion-env)
