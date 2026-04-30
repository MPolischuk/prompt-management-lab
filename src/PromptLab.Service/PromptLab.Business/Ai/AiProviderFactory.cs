using Microsoft.Extensions.Options;
using PromptLab.Business.Ai.Contracts;
using PromptLab.Business.Configuration;

namespace PromptLab.Business.Ai;

public class AiProviderFactory(IEnumerable<IAiProvider> providers, IOptions<AiOptions> options)
{
    public IReadOnlyCollection<string> GetConfiguredProviderNames()
    {
        return providers.Select(p => p.Name).ToArray();
    }

    public IAiProvider? Resolve(string? providerName)
    {
        var selected = string.IsNullOrWhiteSpace(providerName) ? options.Value.DefaultProvider : providerName;
        return providers.FirstOrDefault(p => p.Name.Equals(selected, StringComparison.OrdinalIgnoreCase));
    }
}
