using System.Data;
using PromptLab.Data.Infrastructure;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.Prompts;
using RepoDb;

namespace PromptLab.Data.Repositories;

public class PromptVersionRepository(IDbConnectionFactory connectionFactory) : IPromptVersionRepository
{
    public async Task<IReadOnlyList<PromptVersion>> GetByPromptIdAsync(Guid promptId, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<PromptVersion>(
            StoredProcedures.PromptVersionGetByPromptId,
            new { PromptId = promptId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return rows.ToList();
    }
}
