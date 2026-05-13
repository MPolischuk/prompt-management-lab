---
sidebar_position: 2
title: Cobertura por archivo
---

# Cobertura por archivo

Resumen de los **6** archivos de test y lo que verifican.

## `PromptServiceTests`

| Test | Objetivo |
|------|----------|
| `CreateAsync_WhenSuccessfulWithTags_SetsTagsAndInvalidatesCache` | Creación con tags, invalidación de caché de búsqueda y llamada a `SetTagsAsync`. |
| `CreateAsync_WhenRepositoryFails_DoesNotInvalidateCache` | Fallo en repositorio: sin invalidación ni `SetTagsAsync`. |
| `UpdateAsync_WhenSuccessful_UpdatesTagsAndInvalidatesCache` | Actualización exitosa con tags y caché. |
| `SearchAsync_WhenCached_DoesNotCallRepositoryTwice` | Segunda búsqueda idéntica usa caché. |
| `SetTagsAsync_WhenSuccessful_InvalidatesSearchCache` | `SetTagsAsync` exitoso limpia caché de búsqueda. |

## `TagServiceTests`

| Test | Objetivo |
|------|----------|
| `GetAllAsync_UsesCache_AfterFirstCall` | Segunda lectura global usa caché. |
| `SearchAsync_WhenQueryIsEmpty_DelegatesToGetAll` | Query vacía → `GetAllAsync`. |
| `CreateAsync_WhenFails_DoesNotInvalidateCache` | Fallo al crear: caché intacta. |
| `CreateAsync_WhenSuccessful_InvalidatesCache` | Éxito al crear: siguiente `GetAllAsync` relee repositorio. |
| `SearchAsync_WhenQueryHasValue_UsesRepositorySearch` | Query con valor → `SearchAsync` del repo. |

## `AnalyzeServiceTests`

| Test | Objetivo |
|------|----------|
| `AnalyzeAsync_WhenPromptDoesNotExist_ReturnsFailure` | Prompt inexistente. |
| `AnalyzeAsync_WhenProviderExists_PersistsRun` | Flujo feliz con persistencia. |
| `AnalyzeAsync_WhenModelDoesNotBelongToProvider_ReturnsFailure` | Modelo inválido para proveedor. |
| `AnalyzeAsync_WhenProviderIsNotRegistered_ReturnsUnavailableError` | Proveedor no registrado. |
| `AnalyzeAsync_UsesPromptTargetModel_WhenRequestModelIsMissing` | Hereda `TargetModelId` del prompt. |
| `AnalyzeAsync_UsesPromptModelHint_WhenTargetModelIsMissing` | Usa `ModelHint` si falta target. |
| `AnalyzeAsync_PersistsPromptHashAndEffectiveSettings` | Hash SHA256 del contenido y settings persistidos. |
| `GetProvidersAsync_UsesCache_AndCatalogProviders` | Caché y mezcla catálogo vs habilitados. |
| `GetModelsAsync_UsesCache` | Caché de modelos. |
| `GetProvidersAsync_WhenVendorProviderRegistered_MarksCatalogEntryEnabled` | Proveedor registrado habilita entrada de catálogo. |

## `AiOptionsValidatorTests`

| Test | Objetivo |
|------|----------|
| `Validate_WhenDefaultProviderMissingInEnabledProviders_ReturnsFailure` | `DefaultProvider` debe estar en `EnabledProviders`. |
| `Validate_WhenModelUsesDisabledProvider_ReturnsFailure` | Modelo no puede usar proveedor deshabilitado. |
| `Validate_WhenConfigurationIsConsistent_ReturnsSuccess` | Configuración válida. |

## `AiProviderImplementationTests`

| Test | Objetivo |
|------|----------|
| `GptAiProvider_WhenMock_ReturnsMockPrefix` | Modo mock OpenAI. |
| `GptAiProvider_WhenNotMockAndMissingKey_ReturnsFailed` | Falta API key en modo real. |
| `GptAiProviderRequestClient_ParsesOutputText_AndSendsBearer` | Cliente HTTP OpenAI. |
| `ClaudeAiProvider_WhenMock_ReturnsMockPrefix` | Mock Anthropic. |
| `ClaudeAiProviderRequestClient_SendsHeaders_AndParsesContent` | Cabeceras y parseo Anthropic. |
| `GeminiAiProvider_WhenMock_ReturnsMockPrefix` | Mock Google. |
| `GeminiAiProviderRequestClient_UsesGenerateContentUrl_AndParsesCandidates` | URL y parseo Gemini. |

## `ArchitectureDependencyTests`

| Test | Objetivo |
|------|----------|
| `PromptLabBusiness_ShouldNotReferencePromptLabDataProject` | `PromptLab.Business.csproj` no referencia `PromptLab.Data`. |

## Volver

- [Estrategia](./01-estrategia)
