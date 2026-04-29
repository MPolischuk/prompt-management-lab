# PromptLab - Software Design Document

## 1. Objetivo

PromptLab sera un sistema para gestionar prompts reutilizables. Permitira crear, editar, listar, filtrar, buscar y etiquetar prompts. Tambien incluira un modulo `Analyze` para ejecutar un prompt existente contra un proveedor de IA configurable.

La primera version no incluira autenticacion ni autorizacion. La arquitectura dejara las responsabilidades separadas para poder incorporar seguridad, proveedores reales de IA y nuevas entidades sin reestructurar la solucion.

## 2. Estructura De Proyectos

La solucion se organizara como monorepo:

- `PromptLab.App`: frontend Next.js con TypeScript.
- `PromptLab.Service`: backend .NET 10, solucion y proyectos por capas.
- `PromptLab.DB`: SQL Server Database Project para generar y deployar DACPAC.
- `tests`: pruebas unitarias de frontend y backend.

Estructura esperada:

```text
PromptLab/
  PromptLab.App/
  PromptLab.Service/
    PromptLab.sln
    PromptLab.Entities/
    PromptLab.Data/
    PromptLab.Business/
    PromptLab.Service/
  PromptLab.DB/
    PromptLab.DB.sqlproj
  tests/
    backend/
    frontend/
  SDD/
    PromptLab-SDD.md
```

## 3. Modelo Funcional

### 3.1 Prompt

Parametros sugeridos:

- `id`: identificador unico.
- `title`: titulo visible.
- `description`: descripcion opcional.
- `content`: contenido del prompt.
- `category`: categoria funcional.
- `language`: idioma del prompt.
- `modelHint`: sugerencia de modelo.
- `targetModelId`: modelo objetivo para el que el prompt fue pensado (`gpt-5.5`, `claude-sonnet`, etc). No limita la ejecucion en otros modelos.
- `temperature`: temperatura sugerida para ejecucion.
- `maxTokens`: maximo de tokens sugerido.
- `topP`: valor sugerido de muestreo nucleus.
- `isActive`: estado logico.
- `createdAt`: fecha de creacion.
- `updatedAt`: fecha de ultima modificacion.

El titulo del prompt debera ser unico para evitar ambiguedad en busquedas, seleccion y auditoria de corridas.

### 3.2 Tags

Los tags seran entidades normalizadas y se asignaran a prompts mediante una relacion muchos-a-muchos.

Entidades principales:

- `Tags`
- `PromptTags`

### 3.3 Busqueda Y Filtros

El listado de prompts soportara:

- Texto libre.
- Tags.
- Categoria.
- Idioma.
- Estado.
- Rango de fechas.
- Paginacion.
- Ordenamiento.

La paginacion sera obligatoria para evitar listados completos en memoria.

### 3.4 Analyze

El modulo `Analyze` permitira:

- Seleccionar un prompt existente.
- Enviar variables o input de usuario.
- Elegir proveedor y/o modelo especifico.
- Ejecutar el analisis mediante una interfaz comun.
- Persistir el resultado y metadata de ejecucion.

La ejecucion resolvera parametros efectivos en este orden:

1. Overrides de la corrida (`AnalyzeRequest`).
2. Defaults del prompt.
3. Defaults del modelo.
4. Defaults globales/proveedor.

La primera implementacion del proveedor sera simulada. El diseno debera permitir agregar implementaciones reales para OpenAI, Claude, Gemini u otros proveedores.

## 4. Arquitectura Backend

El backend usara .NET 10 y se organizara en capas:

- `PromptLab.Entities`: entidades, DTOs, contratos de requests/responses, modelos de paginacion y modelos reutilizables de resultados de SPs.
- `PromptLab.Data`: interfaces de repositorios e implementaciones con RepoDB. Esta capa solo invocara stored procedures y functions.
- `PromptLab.Business`: reglas de negocio, validaciones, cache, servicios de dominio y estrategia de proveedores IA.
- `PromptLab.Service`: ASP.NET Core Web API, OpenAPI, controllers/endpoints, dependency injection y configuracion.

### 4.1 Patrones

- Repository: abstrae acceso a datos por interfaces.
- Service Layer: encapsula casos de uso.
- Strategy: seleccion de proveedor IA mediante `IAiProvider`.
- Options Pattern: configuracion tipada desde `appsettings.json` y `appsettings.Development.json`.
- DTO Mapping explicito: separa contratos API, dominio y datos.
- Dependency Injection: todas las dependencias se resolveran por interfaces.

### 4.2 Configuracion

El proyecto API usara:

- `appsettings.json`: configuracion base.
- `appsettings.Development.json`: configuracion local/desarrollo.

Configuraciones previstas:

- Connection strings SQL Server.
- TTL y claves base de MemoryCache.
- Proveedores IA disponibles.
- Proveedor IA default.
- Opciones de OpenAPI.

### 4.3 Cache

Se usara `IMemoryCache` para lecturas frecuentes:

- Tags.
- Proveedores IA configurados.
- Busquedas comunes de prompts con TTL corto.

Las mutaciones de prompts y tags invalidaran las entradas relacionadas.

### 4.4 Modelos Reutilizables Para SPs

Las respuestas comunes de stored procedures se mapearan a modelos compartidos y agnosticos a la entidad. Ejemplos:

- `SaveResult`
- `UpdateResult`
- `DeleteResult`
- `OperationResult`
- `PagedResult<T>`

Esto evita duplicar modelos para resultados tecnicos equivalentes entre prompts, tags y analisis.

## 5. Base De Datos

`PromptLab.DB` sera un SQL Server Database Project compatible con generacion y deploy de DACPAC.

### 5.1 Tablas

Tablas iniciales:

- `Prompts`
- `Tags`
- `PromptTags`
- `AnalysisRuns`

### 5.2 Indices

Indices previstos:

- `Prompts.Title`
- `Prompts.Category`
- `Prompts.Language`
- `Prompts.IsActive`
- `Prompts.CreatedAt`
- `Tags.Slug`
- `PromptTags.PromptId`
- `PromptTags.TagId`
- `AnalysisRuns.PromptId`
- `AnalysisRuns.CreatedAt`

### 5.3 Stored Procedures

SPs sugeridos:

- `dbo.Prompt_Create`
- `dbo.Prompt_Update`
- `dbo.Prompt_Delete`
- `dbo.Prompt_GetById`
- `dbo.Prompt_Search`
- `dbo.Prompt_SetTags`
- `dbo.Tag_Create`
- `dbo.Tag_GetAll`
- `dbo.Tag_Search`
- `dbo.Analyze_CreateRun`
- `dbo.Analyze_GetRunById`

### 5.4 Functions

Functions sugeridas:

- `dbo.fn_NormalizeSlug`
- `dbo.fn_HasPromptTag`

Las functions se usaran para logica reutilizable de SQL, evitando duplicacion dentro de stored procedures.

## 6. APIs OpenAPI

La API expondra endpoints versionados bajo `/api/v1`.

### 6.1 Prompts

- `GET /api/v1/prompts`
- `GET /api/v1/prompts/{id}`
- `POST /api/v1/prompts`
- `PUT /api/v1/prompts/{id}`
- `DELETE /api/v1/prompts/{id}`
- `PUT /api/v1/prompts/{id}/tags`

La eliminacion inicial sera baja logica mediante `isActive = false`.

### 6.2 Tags

- `GET /api/v1/tags`
- `POST /api/v1/tags`

### 6.3 Catalogo IA

- `GET /api/v1/ai-providers`
- `GET /api/v1/ai-models`

### 6.4 Analyze

- `POST /api/v1/analyze`
- `GET /api/v1/analyze/{id}`

## 7. Proveedores Y Modelos IA

Se definira una interfaz comun:

```csharp
public interface IAiProvider
{
    string Name { get; }
    Task<AnalyzeResult> AnalyzeAsync(AnalyzeRequest request, CancellationToken cancellationToken);
}
```

La primera version incluira una implementacion simulada:

- `SimulatedAiProvider`

El selector de proveedor resolvera la implementacion por nombre configurado, permitiendo sumar implementaciones reales luego sin cambiar los casos de uso.

Adicionalmente, se modelara un catalogo explicito de modelos (`AiModel`) para separar:

- `Provider`: vendor/canal de ejecucion.
- `ModelId`: modelo concreto del proveedor.

Los modelos podran declararse como deshabilitados para mostrar roadmap sin anunciar disponibilidad de ejecucion real.

## 8. Frontend

`PromptLab.App` usara Next.js con TypeScript y App Router.

### 8.1 Pantallas

- Listado de prompts con busqueda, filtros y paginacion.
- Crear prompt.
- Editar prompt.
- Detalle de prompt.
- Gestion simple de tags.
- Pantalla `Analyze`.

### 8.2 Componentes

Componentes sugeridos:

- `PromptList`
- `PromptFilters`
- `PromptForm`
- `PromptDetails`
- `TagSelector`
- `Pagination`
- `ProviderSelector`
- `AnalyzeForm`
- `AnalyzeResultPanel`

### 8.3 Cliente API

El cliente API vivira en:

```text
PromptLab.App/src/lib/api
```

Debera centralizar:

- URL base.
- Tipos TypeScript.
- Manejo de errores HTTP.
- Funciones para prompts, tags, proveedores y analyze.

## 9. Tests

### 9.1 Backend

Frameworks sugeridos:

- xUnit.
- Moq.
- FluentAssertions.

Cobertura inicial:

- Servicios de negocio de prompts.
- Validaciones de prompts.
- Invalidacion de cache.
- Selector de proveedores IA.
- Proveedor IA simulado.
- Mapeo de resultados genericos de SPs.

### 9.2 Frontend

Frameworks sugeridos:

- Vitest.
- Testing Library.

Cobertura inicial:

- Render del listado.
- Aplicacion de filtros.
- Formulario de prompt.
- Selector de tags.
- Flujo basico de `Analyze`.

## 10. Performance Y Mantenibilidad

Decisiones de performance:

- Paginacion obligatoria en listados.
- Indices alineados con filtros principales.
- SPs para operaciones funcionales.
- Cache de lecturas frecuentes con TTL corto.
- Invalidacion explicita de cache en mutaciones.
- Evitar SQL dinamico en la capa de datos.

Decisiones de mantenibilidad:

- Clases separadas por archivo.
- Interfaces para servicios y repositorios.
- Capas con responsabilidades claras.
- Contratos reutilizables para resultados genericos.
- Sin Entity Framework.
- RepoDB como ORM/micro ORM.
- OpenAPI como contrato visible de la API.

## 11. Plan De Implementacion

### Fase 1: Base De Datos

1. Crear la carpeta `PromptLab.DB`.
2. Crear `PromptLab.DB.sqlproj`.
3. Definir tablas `Prompts`, `Tags`, `PromptTags` y `AnalysisRuns`.
4. Agregar primary keys, foreign keys, defaults y constraints.
5. Crear indices para busqueda, filtros y joins por tags.
6. Crear functions reutilizables como `fn_NormalizeSlug`.
7. Crear SPs de prompts, tags y analyze.
8. Agregar scripts post-deploy si se requieren datos semilla.
9. Validar build del SQL project y generacion de DACPAC.

### Fase 2: Backend

1. Crear la carpeta `PromptLab.Service`.
2. Crear `PromptLab.sln`.
3. Crear proyectos `PromptLab.Entities`, `PromptLab.Data`, `PromptLab.Business` y `PromptLab.Service`.
4. Configurar referencias entre proyectos.
5. Agregar paquetes necesarios: RepoDB, SQL Client, OpenAPI/Swagger, MemoryCache y testing.
6. Crear entidades, DTOs, requests/responses y modelos genericos de resultados de SPs.
7. Implementar repositorios RepoDB que invoquen solo SPs/functions.
8. Implementar servicios de negocio para prompts, tags y analyze.
9. Implementar cache con `IMemoryCache` e invalidacion en mutaciones.
10. Crear interfaz `IAiProvider`, proveedor simulado y selector de proveedores.
11. Configurar `appsettings.json` y `appsettings.Development.json`.
12. Exponer endpoints REST bajo `/api/v1`.
13. Habilitar OpenAPI/Swagger.
14. Agregar tests unitarios de business, cache, proveedores y mapeos.
15. Ejecutar restore, build y test del backend.

### Fase 3: Frontend

1. Crear la carpeta `PromptLab.App`.
2. Crear app Next.js con TypeScript y App Router.
3. Configurar scripts de desarrollo, build y test.
4. Crear tipos TypeScript alineados con contratos API.
5. Implementar cliente API centralizado.
6. Crear layout base y navegacion.
7. Implementar listado de prompts con filtros, busqueda y paginacion.
8. Implementar formularios de crear y editar prompt.
9. Implementar detalle de prompt.
10. Implementar gestion simple de tags.
11. Implementar pantalla `Analyze` con selector de prompt, proveedor e input.
12. Implementar visualizacion de resultado de `Analyze`.
13. Agregar tests unitarios de componentes y flujos principales.
14. Ejecutar lint, build y test del frontend.

### Fase 4: Integracion Y Validacion

1. Validar que el DACPAC pueda generarse correctamente.
2. Validar que los SPs devuelvan contratos compatibles con `PromptLab.Data`.
3. Ejecutar backend contra una base local de desarrollo.
4. Validar Swagger/OpenAPI.
5. Conectar frontend contra backend local.
6. Probar flujo completo: crear prompt, asignar tags, buscar, editar, desactivar y ejecutar `Analyze`.
7. Ejecutar todos los tests.
8. Documentar comandos de ejecucion local en un README.

## 12. Riesgos Y Consideraciones

- .NET 10 puede requerir SDK preview o version instalada especifica segun el entorno.
- SQL Server Database Project puede requerir tooling adicional en Visual Studio.
- RepoDB sobre SPs requiere mantener contratos SQL y C# sincronizados.
- La ausencia de autenticacion es intencional para la primera version, pero los controllers y services deben quedar preparados para incorporar seguridad despues.
- Los proveedores IA reales deberan agregarse como implementaciones nuevas de `IAiProvider`, evitando cambios en los casos de uso.
