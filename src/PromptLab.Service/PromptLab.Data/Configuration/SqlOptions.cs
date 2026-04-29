namespace PromptLab.Data.Configuration;

public class SqlOptions
{
    public const string SectionName = "Sql";
    public string ConnectionString { get; init; } = string.Empty;
}
