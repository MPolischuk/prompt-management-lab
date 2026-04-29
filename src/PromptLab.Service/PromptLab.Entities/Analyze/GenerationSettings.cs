namespace PromptLab.Entities.Analyze;

public class GenerationSettings
{
    public decimal? Temperature { get; init; }
    public int? MaxTokens { get; init; }
    public decimal? TopP { get; init; }
}
