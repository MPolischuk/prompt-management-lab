namespace PromptLab.Business.Configuration;

public class CacheOptions
{
    public const string SectionName = "Cache";
    public int TagsTtlSeconds { get; init; } = 300;
    public int PromptSearchTtlSeconds { get; init; } = 60;
    public int ProvidersTtlSeconds { get; init; } = 300;
}
