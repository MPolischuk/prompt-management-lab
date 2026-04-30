using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PromptLab.Business.Ai;
using PromptLab.Business.Configuration;
using PromptLab.Entities.Analyze;

namespace PromptLab.Business.Services.Analyze;

internal interface IAnalyzeCatalogService
{
    Task<IReadOnlyCollection<AnalyzeProvider>> GetProvidersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<AiModel>> GetModelsAsync(CancellationToken cancellationToken);
}

internal sealed class AnalyzeCatalogService(
    AiProviderFactory providerFactory,
    IOptions<AiOptions> aiOptions,
    IOptions<CacheOptions> cacheOptions,
    IMemoryCache cache) : IAnalyzeCatalogService
{
    private const string ProvidersCacheKey = "analyze:providers";
    private const string ModelsCacheKey = "analyze:models";

    public Task<IReadOnlyCollection<AnalyzeProvider>> GetProvidersAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(ProvidersCacheKey, out IReadOnlyCollection<AnalyzeProvider>? cached) && cached is not null)
        {
            return Task.FromResult(cached);
        }

        var configuredProviders = providerFactory.GetConfiguredProviderNames();
        var modelsByProvider = AnalyzeModelCatalog.GetConfiguredModels(aiOptions.Value)
            .Where(static m => m.Enabled)
            .GroupBy(m => m.Provider, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyCollection<AiModel>)g.Select(AnalyzeModelCatalog.MapModel).ToArray(),
                StringComparer.OrdinalIgnoreCase);

        var catalogProviders = aiOptions.Value.CatalogProviders.Count > 0
            ? aiOptions.Value.CatalogProviders
            : aiOptions.Value.EnabledProviders;

        var providers = catalogProviders
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

        var models = AnalyzeModelCatalog.GetConfiguredModels(aiOptions.Value)
            .Where(static m => m.Enabled)
            .Select(AnalyzeModelCatalog.MapModel)
            .ToArray();

        cache.Set(ModelsCacheKey, models, TimeSpan.FromSeconds(cacheOptions.Value.ProvidersTtlSeconds));
        return Task.FromResult<IReadOnlyCollection<AiModel>>(models);
    }
}
