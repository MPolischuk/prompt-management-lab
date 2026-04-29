using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PromptLab.Business.Ai;
using PromptLab.Business.Configuration;
using PromptLab.Data.Repositories;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;

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

    public async Task<OperationResult> AnalyzeAsync(AnalyzeRequest request, CancellationToken cancellationToken)
    {
        var prompt = await promptRepository.GetByIdAsync(request.PromptId, cancellationToken);
        if (prompt is null)
        {
            return new OperationResult { Success = false, Message = "Prompt not found." };
        }

        var provider = providerFactory.Resolve(request.Provider);
        if (provider is null)
        {
            return new OperationResult { Success = false, Message = "Provider not configured." };
        }

        var execution = await provider.AnalyzeAsync(prompt, request, cancellationToken);

        var persistence = await analyzeRepository.CreateRunAsync(
            new AnalyzeRun
            {
                PromptId = request.PromptId,
                Provider = provider.Name,
                Input = request.Input,
                Output = execution.Output,
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

        var providers = aiOptions.Value.EnabledProviders
            .Select(name => new AnalyzeProvider { Name = name, Enabled = true })
            .ToArray();

        cache.Set(ProvidersCacheKey, providers, TimeSpan.FromSeconds(cacheOptions.Value.ProvidersTtlSeconds));
        return Task.FromResult<IReadOnlyCollection<AnalyzeProvider>>(providers);
    }
}
