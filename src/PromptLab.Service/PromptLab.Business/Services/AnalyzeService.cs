using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PromptLab.Business.Ai;
using PromptLab.Business.Configuration;
using PromptLab.Business.Services.Analyze;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace PromptLab.Business.Services;

/// <summary>
/// Implementa los casos de uso de analisis, resolviendo modelo/proveedor y persistiendo corridas.
/// </summary>
public class AnalyzeService(
    IPromptRepository promptRepository,
    IAnalyzeRepository analyzeRepository,
    AiProviderFactory providerFactory,
    IOptions<AiOptions> aiOptions,
    IOptions<CacheOptions> cacheOptions,
    IMemoryCache cache) : IAnalyzeService
{
    private readonly IAnalyzeModelResolver _modelResolver = new AnalyzeModelResolver(providerFactory, aiOptions);
    private readonly IAnalyzeCatalogService _catalogService = new AnalyzeCatalogService(providerFactory, aiOptions, cacheOptions, cache);

    /// <inheritdoc />
    public async Task<OperationResult> AnalyzeAsync(AnalyzeRequest request, CancellationToken cancellationToken)
    {
        var prompt = await promptRepository.GetByIdAsync(request.PromptId, cancellationToken);
        if (prompt is null)
        {
            return new OperationResult { Success = false, Message = "Prompt not found.", ErrorCode = OperationErrorCode.NotFound };
        }

        var resolutionError = _modelResolver.TryResolve(request, prompt, out var resolution);
        if (resolutionError is not null)
        {
            return resolutionError;
        }

        var provider = resolution!.Provider;
        var selectedModel = resolution.Model;

        var effectiveSettings = BuildEffectiveSettings(request, prompt, selectedModel);
        var executionRequest = new AnalyzeExecutionRequest
        {
            Provider = provider.Name,
            ModelId = selectedModel.Id,
            Input = request.Input,
            EffectiveSettings = effectiveSettings
        };

        var execution = await provider.AnalyzeAsync(prompt, executionRequest, cancellationToken);
        var promptHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(prompt.Content)));

        var persistence = await analyzeRepository.CreateRunAsync(
            new AnalyzeRun
            {
                PromptId = request.PromptId,
                Provider = provider.Name,
                ModelId = selectedModel.Id,
                Input = request.Input,
                Output = execution.Output,
                Temperature = effectiveSettings.Temperature,
                MaxTokens = effectiveSettings.MaxTokens,
                TopP = effectiveSettings.TopP,
                PromptSnapshot = prompt.Content,
                PromptSnapshotHash = promptHash,
                Status = execution.Status,
                ErrorMessage = execution.ErrorMessage,
                LatencyMs = execution.LatencyMs
            },
            cancellationToken);

        return persistence;
    }

    /// <inheritdoc />
    public Task<AnalyzeRun?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return analyzeRepository.GetRunByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<AnalyzeProvider>> GetProvidersAsync(CancellationToken cancellationToken)
    {
        return _catalogService.GetProvidersAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<AiModel>> GetModelsAsync(CancellationToken cancellationToken)
    {
        return _catalogService.GetModelsAsync(cancellationToken);
    }

    private static GenerationSettings BuildEffectiveSettings(
        AnalyzeRequest request,
        Entities.Prompts.Prompt prompt,
        AiModelOption model)
    {
        return new GenerationSettings
        {
            Temperature = request.Settings?.Temperature ?? prompt.Temperature ?? model.DefaultTemperature,
            MaxTokens = request.Settings?.MaxTokens ?? prompt.MaxTokens ?? model.DefaultMaxTokens,
            TopP = request.Settings?.TopP ?? prompt.TopP ?? model.DefaultTopP
        };
    }

}
