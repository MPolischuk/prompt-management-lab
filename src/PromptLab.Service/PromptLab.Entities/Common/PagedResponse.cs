namespace PromptLab.Entities.Common;

public class PagedResponse<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public required int TotalRows { get; init; }
}
