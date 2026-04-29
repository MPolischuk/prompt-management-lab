using PromptLab.Entities.Common;
using PromptLab.Entities.Prompts;

namespace PromptLab.Data.Repositories;

public interface IPromptRepository
{
    Task<OperationResult> CreateAsync(UpsertPromptRequest request, CancellationToken cancellationToken);
    Task<OperationResult> UpdateAsync(Guid id, UpsertPromptRequest request, CancellationToken cancellationToken);
    Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<Prompt?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResponse<Prompt>> SearchAsync(PromptSearchRequest request, CancellationToken cancellationToken);
    Task<OperationResult> SetTagsAsync(Guid promptId, IReadOnlyCollection<Guid> tagIds, CancellationToken cancellationToken);
}
