using System.Data;
using PromptLab.Data.Infrastructure;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.TestCases;
using RepoDb;

namespace PromptLab.Data.Repositories;

public class TestCaseRepository(IDbConnectionFactory connectionFactory) : ITestCaseRepository
{
    public async Task<IReadOnlyList<TestCase>> GetBySuiteIdAsync(Guid suiteId, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<TestCase>(
            StoredProcedures.TestCaseGetBySuiteId,
            new { SuiteId = suiteId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return rows.ToList();
    }

    public async Task<OperationResult> CreateAsync(CreateTestCaseRequest request, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.TestCaseCreate,
            new { request.SuiteId, request.Name, request.InputVariables, request.ExpectedOutput },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }

    public async Task<OperationResult> UpdateAsync(Guid id, UpdateTestCaseRequest request, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.TestCaseUpdate,
            new { Id = id, request.Name, request.InputVariables, request.ExpectedOutput },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }

    public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.TestCaseDelete,
            new { Id = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }
}
