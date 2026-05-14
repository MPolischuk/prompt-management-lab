using System.Data;
using PromptLab.Data.Infrastructure;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.TestResults;
using PromptLab.Entities.TestRuns;
using RepoDb;

namespace PromptLab.Data.Repositories;

public class TestRunRepository(IDbConnectionFactory connectionFactory) : ITestRunRepository
{
    public async Task<IReadOnlyList<TestRun>> GetAllAsync(CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<TestRun>(
            StoredProcedures.TestRunGetAll,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return rows.ToList();
    }

    public async Task<IReadOnlyList<TestRun>> GetBySuiteIdAsync(Guid suiteId, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<TestRun>(
            StoredProcedures.TestRunGetBySuiteId,
            new { SuiteId = suiteId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return rows.ToList();
    }

    public async Task<TestRun?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<TestRun>(
            StoredProcedures.TestRunGetById,
            new { Id = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return rows.FirstOrDefault();
    }

    public async Task<OperationResult> CreateAsync(CreateTestRunRequest request, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.TestRunCreate,
            new
            {
                request.SuiteId,
                request.PromptId,
                request.PromptVersion,
                request.Model,
                request.Temperature,
                request.MaxTokens,
                request.Status
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }

    public async Task<OperationResult> UpdateAsync(Guid id, UpdateTestRunRequest request, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.TestRunUpdate,
            new { Id = id, request.Status, request.StartedAt, request.CompletedAt },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }

    public async Task<OperationResult> CreateResultAsync(CreateTestResultRequest request, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.TestResultCreate,
            new
            {
                request.RunId,
                request.CaseId,
                request.ActualOutput,
                request.Passed,
                request.Score,
                request.LatencyMs,
                request.Error
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return result.First();
    }

    public async Task<IReadOnlyList<TestResult>> GetResultsByRunIdAsync(Guid runId, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<TestResult>(
            StoredProcedures.TestResultGetByRunId,
            new { RunId = runId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return rows.ToList();
    }
}
