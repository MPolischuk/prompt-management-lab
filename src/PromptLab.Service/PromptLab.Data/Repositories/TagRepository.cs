using System.Data;
using PromptLab.Data.Infrastructure;
using PromptLab.Entities.Common;
using PromptLab.Entities.Tags;
using RepoDb;

namespace PromptLab.Data.Repositories;

public class TagRepository(IDbConnectionFactory connectionFactory) : ITagRepository
{
    public async Task<IReadOnlyCollection<Tag>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<Tag>(
            StoredProcedures.TagGetAll,
            commandType: CommandType.StoredProcedure);

        return rows.ToList();
    }

    public async Task<IReadOnlyCollection<Tag>> SearchAsync(string? query, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<Tag>(
            StoredProcedures.TagSearch,
            new { Query = query },
            commandType: CommandType.StoredProcedure);

        return rows.ToList();
    }

    public async Task<OperationResult> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.TagCreate,
            new { request.Name },
            commandType: CommandType.StoredProcedure);

        return rows.First();
    }
}
