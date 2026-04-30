using Microsoft.Extensions.Options;

namespace PromptLab.Business.Configuration;

public sealed class AiOptionsValidator : IValidateOptions<AiOptions>
{
    public ValidateOptionsResult Validate(string? name, AiOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.DefaultProvider))
        {
            failures.Add("Ai:DefaultProvider is required.");
        }

        var enabledProviders = options.EnabledProviders
            .Where(static p => !string.IsNullOrWhiteSpace(p))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (enabledProviders.Count == 0)
        {
            failures.Add("Ai:EnabledProviders must contain at least one provider.");
        }

        if (!string.IsNullOrWhiteSpace(options.DefaultProvider) &&
            !enabledProviders.Contains(options.DefaultProvider))
        {
            failures.Add("Ai:DefaultProvider must exist in Ai:EnabledProviders.");
        }

        foreach (var model in options.Models.Where(static m => !string.IsNullOrWhiteSpace(m.Provider)))
        {
            if (!enabledProviders.Contains(model.Provider))
            {
                failures.Add($"Ai:Models provider '{model.Provider}' is not listed in Ai:EnabledProviders.");
            }
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
