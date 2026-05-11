using System.Data;
using PromptLab.Data.Infrastructure;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.TestSuites;
using RepoDb;

namespace PromptLab.Data.Repositories;

public class TestSuiteRepository(IDbConnectionFactory connectionFactory) : ITestSuiteRepository
{
    public async Task<IReadOnlyList<TestSuite>> GetByPromptIdAsync(Guid promptId, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<TestSuite>(
            StoredProcedures.TestSuiteGetByPromptId,
            new { PromptId = promptId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return rows.ToList();
    }

    public async Task<TestSuite?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<TestSuite>(
            StoredProcedures.TestSuiteGetById,
            new { Id = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return rows.FirstOrDefault();
    }

    public async Task<OperationResult> CreateAsync(CreateTestSuiteRequest request, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.TestSuiteCreate,
            new { request.PromptId, request.Name, request.Description },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }

    public async Task<OperationResult> UpdateAsync(Guid id, UpdateTestSuiteRequest request, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.TestSuiteUpdate,
            new { Id = id, request.Name, request.Description },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }

    public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.TestSuiteDelete,
            new { Id = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }
}
