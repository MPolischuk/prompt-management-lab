using System.Data;
using PromptLab.Data.Infrastructure;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using RepoDb;

namespace PromptLab.Data.Repositories;

public class AnalyzeRepository(IDbConnectionFactory connectionFactory) : IAnalyzeRepository
{
    public async Task<OperationResult> CreateRunAsync(AnalyzeRun run, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<OperationResult>(
            StoredProcedures.AnalyzeCreateRun,
            new
            {
                run.PromptId,
                run.Provider,
                run.ModelId,
                run.Input,
                run.Output,
                run.Temperature,
                run.MaxTokens,
                run.TopP,
                run.PromptSnapshot,
                run.PromptSnapshotHash,
                run.Status,
                run.ErrorMessage,
                run.LatencyMs
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return rows.First();
    }

    public async Task<AnalyzeRun?> GetRunByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.ExecuteQueryAsync<AnalyzeRun>(
            StoredProcedures.AnalyzeGetRunById,
            new { Id = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return rows.FirstOrDefault();
    }
}
