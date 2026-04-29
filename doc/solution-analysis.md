# Solution Analysis

## Summary

Analisis de `prompt-management-lab.sln`, que incluye 5 proyectos: 4 proyectos .NET de backend y 1 proyecto de tests. La arquitectura actual separa host web, negocio, datos y entidades, pero la capa `PromptLab.Business` depende directamente de `PromptLab.Data`, incluyendo interfaces de repositorio definidas dentro del proyecto de infraestructura. Tambien existe un proyecto SQL (`src/PromptLab.DB/PromptLab.DB.sqlproj`) con tablas y stored procedures usados por `PromptLab.Data`, pero no esta incluido en la solucion.

Riesgos principales:

- `PromptLab.Business` esta acoplado a `PromptLab.Data`, lo que debilita la separacion entre reglas de negocio e infraestructura.
- `PromptLab.DB.sqlproj` queda fuera de `prompt-management-lab.sln`, por lo que el build de solucion no valida cambios de base de datos.
- La cobertura de tests visible se limita a `AnalyzeService`; no hay tests para servicios de prompts/tags, controladores ni repositorios.
- Los repositorios aceptan `CancellationToken`, pero las llamadas RepoDb no lo propagan, reduciendo la capacidad real de cancelar requests.

## Project Inventory

### Proyectos incluidos en `prompt-management-lab.sln`

| Proyecto | Ruta | Rol observado | Framework |
| --- | --- | --- | --- |
| `PromptLab.Service` | `src/PromptLab.Service/PromptLab.Service/PromptLab.Service.csproj` | Host ASP.NET Core Web API, controladores, DI, Swagger, CORS y versionado | `net10.0` |
| `PromptLab.Business` | `src/PromptLab.Service/PromptLab.Business/PromptLab.Business.csproj` | Servicios de aplicacion/negocio, cache, resolucion de proveedores AI | `net10.0` |
| `PromptLab.Data` | `src/PromptLab.Service/PromptLab.Data/PromptLab.Data.csproj` | Infraestructura SQL, repositorios, RepoDb, connection factory | `net10.0` |
| `PromptLab.Entities` | `src/PromptLab.Service/PromptLab.Entities/PromptLab.Entities.csproj` | Entidades, DTOs y modelos compartidos | `net10.0` |
| `PromptLab.Business.Tests` | `src/tests/backend/PromptLab.Business.Tests/PromptLab.Business.Tests.csproj` | Tests unitarios de negocio con xUnit, Moq y FluentAssertions | `net10.0` |

### Proyecto relacionado no incluido en la solucion

| Proyecto | Ruta | Rol observado |
| --- | --- | --- |
| `PromptLab.DB` | `src/PromptLab.DB/PromptLab.DB.sqlproj` | Modelo SSDT SQL Server con tablas, funciones, stored procedures y post-deploy |

## Dependency Graph

Dependencias de proyectos:

```text
PromptLab.Service
  -> PromptLab.Business
  -> PromptLab.Data
  -> PromptLab.Entities

PromptLab.Business
  -> PromptLab.Data
  -> PromptLab.Entities

PromptLab.Data
  -> PromptLab.Entities

PromptLab.Business.Tests
  -> PromptLab.Business
  -> PromptLab.Entities

PromptLab.Entities
  -> ninguna referencia de proyecto
```

Paquetes relevantes:

- `PromptLab.Service`: `Asp.Versioning.Mvc`, `Asp.Versioning.Mvc.ApiExplorer`, `Microsoft.AspNetCore.OpenApi`, `Swashbuckle.AspNetCore`.
- `PromptLab.Business`: `Microsoft.Extensions.Caching.Memory`, `Microsoft.Extensions.Configuration.Abstractions`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`, `Microsoft.Extensions.Options.ConfigurationExtensions`.
- `PromptLab.Data`: `Microsoft.Data.SqlClient`, `RepoDb.SqlServer`, abstractions de configuracion/DI/options.
- `PromptLab.Business.Tests`: `xunit`, `xunit.runner.visualstudio`, `Moq`, `FluentAssertions`, `coverlet.collector`, `Microsoft.NET.Test.Sdk`.

## Related Projects

- `PromptLab.Service` compone la aplicacion en `Program.cs` con `AddPromptLabData(builder.Configuration)` y `AddPromptLabBusiness(builder.Configuration)`.
- `PromptLab.Business` consume repositorios de `PromptLab.Data.Repositories` desde `AnalyzeService`, `PromptService` y `TagService`.
- `PromptLab.Data` implementa persistencia SQL mediante stored procedures declarados en `StoredProcedures.cs` y definidos en `src/PromptLab.DB`.
- `PromptLab.Business.Tests` cubre parcialmente `AnalyzeService`; no hay proyectos de tests para `PromptLab.Service`, `PromptLab.Data`, `PromptLab.Entities` ni `PromptLab.DB`.

## Architecture Issues

### 1. Capa de negocio depende directamente de infraestructura

- Severity: `High`
- Evidence: `PromptLab.Business.csproj` referencia `..\PromptLab.Data\PromptLab.Data.csproj`; `AnalyzeService.cs`, `PromptService.cs` y `TagService.cs` importan `PromptLab.Data.Repositories`.
- Risk: las reglas de negocio quedan atadas a un proyecto de infraestructura. Esto dificulta sustituir persistencia, probar servicios sin conocer el proyecto de datos y mantener una direccion clara de dependencias.
- Recommendation: mover las interfaces `IPromptRepository`, `ITagRepository` e `IAnalyzeRepository` a `PromptLab.Business` o a un proyecto de abstracciones/application contracts. Luego dejar `PromptLab.Data` como implementacion que referencia esas abstracciones.

### 2. Proyecto de base de datos no esta incluido en la solucion

- Severity: `Medium`
- Evidence: `src/PromptLab.DB/PromptLab.DB.sqlproj` existe y contiene `Tables`, `Functions`, `StoredProcedures` y `PostDeploy`, pero `prompt-management-lab.sln` solo incluye proyectos `PromptLab.Service`, `PromptLab.Business`, `PromptLab.Data`, `PromptLab.Entities` y `PromptLab.Business.Tests`.
- Risk: cambios de schema/stored procedures pueden quedar fuera del ciclo normal de build/review de la solucion. `PromptLab.Data` depende de esos objetos por nombre, pero la solucion no valida que sigan existiendo.
- Recommendation: incluir `PromptLab.DB.sqlproj` en la solucion o documentar/buildar el proyecto SQL en un pipeline separado si SSDT no esta disponible en todos los entornos.

### 3. Host web referencia `PromptLab.Data` directamente

- Severity: `Medium`
- Evidence: `PromptLab.Service.csproj` referencia `PromptLab.Data` y `Program.cs` invoca `AddPromptLabData`.
- Risk: esta referencia es aceptable como composition root, pero combinada con la dependencia `Business -> Data` deja la infraestructura accesible desde mas capas de las necesarias.
- Recommendation: mantener la referencia directa solo en el host para DI, y eliminar la dependencia de `PromptLab.Business` hacia `PromptLab.Data` moviendo los contratos.

### 4. Configuracion de proveedores AI lista proveedores no implementados

- Severity: `Low`
- Evidence: `appsettings.json` y `appsettings.Development.json` habilitan nombres `openai`, `anthropic` y `google`, pero `ServiceCollectionExtensions.cs` solo registra `SimulatedAiProvider`.
- Risk: la API puede devolver proveedores configurados como no disponibles, lo cual parece intencional por el campo `Enabled`, pero puede confundir si se interpreta `EnabledProviders` como integraciones realmente activas.
- Recommendation: renombrar la configuracion para distinguir catalogo vs proveedores implementados, o registrar stubs/adaptadores explicitos con estado claro.

## Code Quality Findings

### 1. Interfaces de repositorio viven en el namespace de infraestructura

- Severity: `High`
- Evidence: `IPromptRepository.cs`, `ITagRepository.cs` e `IAnalyzeRepository.cs` estan en `PromptLab.Data.Repositories`; los servicios de negocio dependen de esas interfaces.
- Risk: aunque se usen interfaces, la abstraccion pertenece al detalle de infraestructura. Esto reduce el valor de inversion de dependencias.
- Recommendation: ubicar los contratos cerca de los casos de uso que los consumen, por ejemplo `PromptLab.Business.Repositories` o `PromptLab.Business.Abstractions`.

### 2. `CancellationToken` no se propaga a operaciones de base de datos

- Severity: `Medium`
- Evidence: `PromptRepository`, `TagRepository` y `AnalyzeRepository` reciben `CancellationToken`, pero llaman `ExecuteQueryAsync` sin pasarlo.
- Risk: requests cancelados desde ASP.NET Core pueden seguir ejecutando consultas o stored procedures hasta finalizar, consumiendo conexiones y recursos.
- Recommendation: revisar si la version de RepoDb usada soporta overloads con token. Si no, documentar la limitacion o encapsular llamadas en una API propia que permita cancelacion cuando se migre.

### 3. Invalidacion de cache ocurre antes de confirmar exito

- Severity: `Low`
- Evidence: `PromptService.CreateAsync`, `UpdateAsync`, `DeleteAsync` y `SetTagsAsync` llaman `InvalidatePromptSearchCache()` antes o independientemente del resultado exitoso.
- Risk: no rompe consistencia funcional, pero genera churn de cache en operaciones fallidas y puede ocultar errores de flujo.
- Recommendation: invalidar solo cuando `OperationResult.Success` sea `true`, salvo que exista un motivo explicito para limpiar ante fallos.

### 4. Controladores devuelven status codes poco diferenciados

- Severity: `Low`
- Evidence: `PromptsController.UpdateAsync` y `DeleteAsync` convierten cualquier resultado no exitoso en `NotFound`; `AnalyzeController.AnalyzeAsync` convierte cualquier fallo en `BadRequest`.
- Risk: errores de validacion, conflicto, dependencia externa o persistencia quedan mezclados bajo el mismo status code.
- Recommendation: enriquecer `OperationResult` con un codigo de error o categoria para mapear a `400`, `404`, `409` o `500` segun corresponda.

## Test Coverage Gaps

- Severity: `Medium`
- Evidence: solo existe `src/tests/backend/PromptLab.Business.Tests/AnalyzeServiceTests.cs`.
- Risk: `PromptService`, `TagService`, controladores, configuracion DI y repositorios SQL no tienen cobertura visible. Cambios en cache, tags, paginacion, stored procedure parameters o routing pueden romperse sin deteccion temprana.
- Recommendation: agregar tests unitarios para `PromptService` y `TagService`, tests de controladores o endpoint-level para `PromptLab.Service`, y tests de integracion para repositorios contra una base efimera o un contrato SQL controlado.

Tests existentes cubren:

- prompt inexistente en `AnalyzeService.AnalyzeAsync`;
- persistencia de run cuando el provider existe;
- rechazo de modelo cuando no pertenece al provider seleccionado.

Brechas concretas en `AnalyzeService`:

- resolucion por `TargetModelId`, `ModelHint` y fallback al primer modelo;
- cache de providers/models;
- error cuando no hay modelo configurado;
- error cuando provider no esta registrado;
- persistencia de `PromptSnapshotHash` y settings efectivos.

## Duplication Areas

- `appsettings.json` y `appsettings.Development.json` duplican casi toda la configuracion `Ai` y `Cache`, cambiando principalmente connection string y TTLs.
- `AnalyzeServiceTests` repite construccion de mocks, `MemoryCache`, `AiOptions` y `AnalyzeService`; conviene extraer helpers/factory de test si la suite crece.
- Los repositorios repiten el patron `CreateConnection` + `ExecuteQueryAsync` + `First/FirstOrDefault`; puede mantenerse por ahora, pero si aumentan los repositorios conviene centralizar manejo de errores/resultados.

## Recommendations

1. Separar contratos de repositorio de `PromptLab.Data` para que `PromptLab.Business` no dependa de infraestructura.
2. Agregar `PromptLab.DB.sqlproj` a la solucion o definir un build SQL separado y visible en CI.
3. Ampliar tests de negocio antes de refactors, empezando por `PromptService`, `TagService` y paths faltantes de `AnalyzeService`.
4. Revisar la propagacion real de `CancellationToken` en RepoDb y el manejo de status codes desde `OperationResult`.
5. Mantener `PromptLab.Service` como composition root, pero evitar que controladores o negocio consuman implementaciones concretas de datos.

## Next Steps

1. Ejecutar `dotnet test prompt-management-lab.sln` para establecer baseline de tests.
2. Crear una rama/refactor corto para mover `I*Repository` fuera de `PromptLab.Data`.
3. Incluir o documentar el build de `src/PromptLab.DB/PromptLab.DB.sqlproj`.
4. Agregar tests faltantes para cache, tags, busqueda de prompts y resolucion de modelos AI.
