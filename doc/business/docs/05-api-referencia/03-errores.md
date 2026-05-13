---
sidebar_position: 3
title: Errores y OperationResult
---

# Errores y `OperationResult`

## Contrato JSON

```json
{
  "success": false,
  "entityId": null,
  "message": "Texto legible",
  "errorCode": "Validation"
}
```

En C#, `errorCode` es el enum **`OperationErrorCode`** serializado como string. En el cliente TypeScript (`apiClient.ts`) se tipa como `string | null`.

## Códigos (`OperationErrorCode`)

| Código | Significado |
|--------|-------------|
| `None` | Sin error (operación exitosa). |
| `Validation` | Entrada o regla de negocio inválida. |
| `NotFound` | Recurso inexistente. |
| `Conflict` | Conflicto de estado o duplicado. |
| `Unavailable` | Dependencia externa o proveedor no disponible. |
| `Unexpected` | Fallo no clasificado. |

## Mapeo HTTP (`OperationResultHttpMapper`)

| `errorCode` | HTTP |
|-------------|------|
| `NotFound` | 404 |
| `Conflict` | 409 |
| `Unavailable` | 503 |
| `Validation` | 400 |
| *(incl. `Unexpected`)* | 400 |

Las operaciones exitosas devuelven **`200 OK`** con el cuerpo `OperationResult` cuando el controlador usa `Ok(result)`; algunas creaciones usan **`201 Created`** con `Location` y el mismo tipo de cuerpo.

## Cliente web

`ApiError` conserva el `status` HTTP y, si existe, el objeto `operation` parseado para mostrar mensajes en UI o logs.

## Volver

- [Overview backend](../03-backend/01-overview)
