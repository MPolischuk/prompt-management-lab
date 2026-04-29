using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PromptLab.Business.Ai;
using PromptLab.Business.Configuration;
using PromptLab.Data.Repositories;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;
using System.Security.Cryptography;
using System.Text;

namespace PromptLab.Business.Services;

public class AnalyzeService(
    IPromptRepository promptRepository,
    IAnalyzeRepository analyzeRepository,
    AiProviderFactory providerFactory,
    IOptions<AiOptions> aiOptions,
    IOptions<CacheOptions> cacheOptions,
    IMemoryCache cache) : IAnalyzeService
{
    private const string ProvidersCacheKey = "analyze:providers";
    private const string ModelsCacheKey = "analyze:models";

    public async Task<OperationResult> AnalyzeAsync(AnalyzeRequest request, CancellationToken cancellationToken)
    {
        var prompt = await promptRepository.GetByIdAsync(request.PromptId, cancellationToken);
        if (prompt is null)
        {
            return new OperationResult { Success = false, Message = "Prompt not found." };
        }

        var modelCatalog = aiOptions.Value.Models
            .Where(m => m.Enabled)
            .ToDictionary(m => m.Id, StringComparer.OrdinalIgnoreCase);

        var selectedModel = ResolveModel(request, prompt, modelCatalog);
        if (selectedModel is null)
        {
            return new OperationResult { Success = false, Message = "Model not configured." };
        }

        var selectedProvider = ResolveProvider(request, selectedModel);
        var provider = providerFactory.Resolve(selectedProvider);
        if (provider is null)
        {
            return new OperationResult { Success = false, Message = "Provider not configured." };
        }

        if (!provider.Name.Equals(selectedModel.Provider, StringComparison.OrdinalIgnoreCase))
        {
            return new OperationResult { Success = false, Message = "Model does not belong to selected provider." };
        }

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

    public Task<AnalyzeRun?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return analyzeRepository.GetRunByIdAsync(id, cancellationToken);
    }

    public Task<IReadOnlyCollection<AnalyzeProvider>> GetProvidersAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(ProvidersCacheKey, out IReadOnlyCollection<AnalyzeProvider>? cached) && cached is not null)
        {
            return Task.FromResult(cached);
        }

        var configuredProviders = providerFactory.GetConfiguredProviderNames();
        var modelsByProvider = aiOptions.Value.Models
            .Where(m => m.Enabled)
            .GroupBy(m => m.Provider, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyCollection<AiModel>)g.Select(MapModel).ToArray(),
                StringComparer.OrdinalIgnoreCase);

        var providers = aiOptions.Value.EnabledProviders
            .Select(name => new AnalyzeProvider
            {
                Name = name,
                Enabled = configuredProviders.Any(p => p.Equals(name, StringComparison.OrdinalIgnoreCase)),
                Models = modelsByProvider.TryGetValue(name, out var models) ? models : []
            })
            .ToArray();

        cache.Set(ProvidersCacheKey, providers, TimeSpan.FromSeconds(cacheOptions.Value.ProvidersTtlSeconds));
        return Task.FromResult<IReadOnlyCollection<AnalyzeProvider>>(providers);
    }

    public Task<IReadOnlyCollection<AiModel>> GetModelsAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(ModelsCacheKey, out IReadOnlyCollection<AiModel>? cached) && cached is not null)
        {
            return Task.FromResult(cached);
        }

        var models = aiOptions.Value.Models
            .Where(m => m.Enabled)
            .Select(MapModel)
            .ToArray();

        cache.Set(ModelsCacheKey, models, TimeSpan.FromSeconds(cacheOptions.Value.ProvidersTtlSeconds));
        return Task.FromResult<IReadOnlyCollection<AiModel>>(models);
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

    private static string? ResolveProvider(AnalyzeRequest request, AiModelOption selectedModel)
    {
        return string.IsNullOrWhiteSpace(request.Provider) ? selectedModel.Provider : request.Provider;
    }

    private static AiModelOption? ResolveModel(
        AnalyzeRequest request,
        Entities.Prompts.Prompt prompt,
        IReadOnlyDictionary<string, AiModelOption> modelCatalog)
    {
        if (!string.IsNullOrWhiteSpace(request.ModelId))
        {
            return modelCatalog.GetValueOrDefault(request.ModelId);
        }

        if (!string.IsNullOrWhiteSpace(prompt.DefaultModelId))
        {
            return modelCatalog.GetValueOrDefault(prompt.DefaultModelId);
        }

        if (!string.IsNullOrWhiteSpace(prompt.ModelHint))
        {
            return modelCatalog.GetValueOrDefault(prompt.ModelHint);
        }

        return modelCatalog.Values.FirstOrDefault();
    }

    private static AiModel MapModel(AiModelOption option)
    {
        return new AiModel
        {
            Id = option.Id,
            Provider = option.Provider,
            DisplayName = option.DisplayName ?? option.Id,
            Enabled = option.Enabled
        };
    }
}
