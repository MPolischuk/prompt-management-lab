namespace PromptLab.Business.Configuration;

public class AiOptions
{
    public const string SectionName = "Ai";
    public string DefaultProvider { get; init; } = "simulated";
    public IReadOnlyCollection<string> EnabledProviders { get; init; } = ["simulated"];
    public IReadOnlyCollection<AiModelOption> Models { get; init; } = [];
}

public class AiModelOption
{
    public required string Id { get; init; }
    public required string Provider { get; init; }
    public string? DisplayName { get; init; }
    public bool Enabled { get; init; } = true;
    public decimal? DefaultTemperature { get; init; }
    public int? DefaultMaxTokens { get; init; }
    public decimal? DefaultTopP { get; init; }
}
