using PromptLab.Entities.Prompts;

namespace PromptLab.Entities.Contracts;

public interface IPromptVersionRepository
{
    Task<IReadOnlyList<PromptVersion>> GetByPromptIdAsync(Guid promptId, CancellationToken cancellationToken);
}
