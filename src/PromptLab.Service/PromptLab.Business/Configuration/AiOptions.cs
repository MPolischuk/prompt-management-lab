namespace PromptLab.Business.Configuration;

public class AiOptions
{
    public const string SectionName = "Ai";
    public string DefaultProvider { get; init; } = "simulated";
    public IReadOnlyCollection<string> EnabledProviders { get; init; } = ["simulated", "openai", "claude", "gemini"];
}
