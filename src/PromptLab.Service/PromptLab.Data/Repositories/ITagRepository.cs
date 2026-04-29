using PromptLab.Entities.Common;
using PromptLab.Entities.Tags;

namespace PromptLab.Data.Repositories;

public interface ITagRepository
{
    Task<IReadOnlyCollection<Tag>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Tag>> SearchAsync(string? query, CancellationToken cancellationToken);
    Task<OperationResult> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken);
}
