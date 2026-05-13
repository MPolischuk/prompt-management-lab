---
sidebar_position: 1
title: Overview del backend
---

# Overview del backend

El backend se organiza en **API** (`PromptLab.Service`) y **dominio** (`PromptLab.Business`), con **persistencia** aislada en `PromptLab.Data` y **contratos** compartidos en `PromptLab.Entities`.

## Convenciones

### `OperationResult`

Muchas operaciones mutables devuelven un **`OperationResult`** JSON con:

- `success`, `entityId`, `message`, `errorCode`

Los controladores traducen fallos a HTTP mediante [`OperationResultHttpMapper`](../05-api-referencia/03-errores).

### Inyección de dependencias

- `Program.cs` registra `AddPromptLabData`, `AddPromptLabBusiness` y `AddPromptLabIntegrations`.
- Los controladores reciben **interfaces** de la capa de negocio (`IPromptService`, `ITestRunService`, etc.).

### Versionado de API

- Todas las rutas bajo `api/*` usan **`[ApiVersion("1.0")]`**.
- El lector de versión es la cabecera **`x-api-version`** (valor `1.0` por defecto si se omite según configuración).

### CORS

Orígenes permitidos en desarrollo: `http://localhost:3000` y `http://localhost:5173` (Vite).

### OpenAPI

Swagger UI habilitado en desarrollo (`UseSwagger` / `UseSwaggerUI`).

## Siguiente

- [Controladores](./02-controladores)
