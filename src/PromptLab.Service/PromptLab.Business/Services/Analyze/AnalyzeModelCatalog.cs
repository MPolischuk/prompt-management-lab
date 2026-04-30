using PromptLab.Business.Configuration;
using PromptLab.Entities.Analyze;

namespace PromptLab.Business.Services.Analyze;

internal static class AnalyzeModelCatalog
{
    internal static IReadOnlyCollection<AiModelOption> GetConfiguredModels(AiOptions options)
    {
        var merged = new Dictionary<string, AiModelOption>(StringComparer.OrdinalIgnoreCase);

        foreach (var model in options.Models)
        {
            if (!string.IsNullOrWhiteSpace(model.Id))
            {
                merged[model.Id] = model;
            }
        }

        MergeProviderModels(merged, "openai", options.Providers.OpenAi.Models);
        MergeProviderModels(merged, "anthropic", options.Providers.Anthropic.Models);
        MergeProviderModels(merged, "google", options.Providers.Google.Models);

        return merged.Values.ToArray();
    }

    internal static AiModel MapModel(AiModelOption option)
    {
        return new AiModel
        {
            Id = option.Id,
            Provider = option.Provider,
            DisplayName = option.DisplayName ?? option.Id,
            Enabled = option.Enabled
        };
    }

    private static void MergeProviderModels(
        IDictionary<string, AiModelOption> target,
        string provider,
        IReadOnlyCollection<AiProviderModelOption> providerModels)
    {
        foreach (var model in providerModels)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                continue;
            }

            target[model.Id] = new AiModelOption
            {
                Id = model.Id,
                Provider = provider,
                DisplayName = model.DisplayName,
                Enabled = model.Enabled,
                DefaultTemperature = model.DefaultTemperature,
                DefaultMaxTokens = model.DefaultMaxTokens,
                DefaultTopP = model.DefaultTopP
            };
        }
    }
}
