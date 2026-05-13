---
sidebar_position: 1
title: Estrategia de pruebas
---

# Estrategia de pruebas

## Alcance actual

Las pruebas automatizadas viven en **`src/tests/backend/PromptLab.Business.Tests`** y se centran en la capa **`PromptLab.Business`**:

- Servicios de dominio (prompts, tags, análisis).
- Validadores de opciones (`AiOptionsValidator`).
- Implementaciones de proveedores de IA con **HTTP simulado** (`HttpMessageHandler` de prueba).
- **Arquitectura**: reglas de referencia entre proyectos `.csproj`.

No hay en este repo una suite E2E del ClientApp ni tests de integración contra SQL Server real en el árbol documentado.

## Herramientas

- **xUnit** (`[Fact]`).
- **Moq** (inferido por el estilo de tests con repositorios simulados).

## Cómo ejecutar

Desde la raíz del repo o la carpeta de tests:

```bash
dotnet test src/tests/backend/PromptLab.Business.Tests/PromptLab.Business.Tests.csproj
```

## Buenas prácticas sugeridas

- Añadir tests cuando se cambie lógica de caché o de `OperationResult`.
- Mantener el test de arquitectura al introducir nuevas referencias de proyecto.

## Siguiente

- [Cobertura por archivo](./02-cobertura)
