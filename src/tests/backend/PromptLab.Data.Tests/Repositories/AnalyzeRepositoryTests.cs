using System.Data;
using FluentAssertions;
using PromptLab.Data.Infrastructure;
using PromptLab.Data.Repositories;
using PromptLab.Data.Tests.Fakes;
using PromptLab.Data.Tests.Infrastructure;
using PromptLab.Entities.Analyze;

namespace PromptLab.Data.Tests.Repositories;

public class AnalyzeRepositoryTests
{
    public AnalyzeRepositoryTests() => RepoDbTestInitializer.EnsureSqlServer();

    [Fact]
    public async Task CreateRunAsync_ExecutesAnalyzeCreateRun()
    {
        var runId = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.AnalyzeCreateRun);
                var p = cmd.GetParameterValues();
                p["PromptId"].Should().NotBeNull();
                p["Provider"].Should().Be("openai");
                return DataTableTestHelpers.OperationResultTable(true, runId);
            }
        };
        var repo = new AnalyzeRepository(new TestDbConnectionFactory(conn));
        var run = new AnalyzeRun
        {
            Id = runId,
            PromptId = Guid.NewGuid(),
            Provider = "openai",
            Status = "Completed",
            CreatedAt = DateTime.UtcNow
        };

        var result = await repo.CreateRunAsync(run, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.EntityId.Should().Be(runId);
    }

    [Fact]
    public async Task CreateRunAsync_PassesAllRequiredParameters()
    {
        var runId = Guid.NewGuid();
        var promptId = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.AnalyzeCreateRun);
                var p = cmd.GetParameterValues();
                p["PromptId"].Should().Be(promptId);
                p["Provider"].Should().Be("anthropic");
                p["ModelId"].Should().Be("claude-3");
                p["Input"].Should().Be("in");
                p["Output"].Should().Be("out");
                p["Temperature"].Should().Be(0.5m);
                p["MaxTokens"].Should().Be(512);
                p["TopP"].Should().Be(0.9m);
                p["PromptSnapshot"].Should().Be("snap");
                p["PromptSnapshotHash"].Should().Be("hash");
                p["Status"].Should().Be("Completed");
                p["ErrorMessage"].Should().Be("err");
                p["LatencyMs"].Should().Be(99);
                return DataTableTestHelpers.OperationResultTable(true, runId);
            }
        };
        var repo = new AnalyzeRepository(new TestDbConnectionFactory(conn));
        var run = new AnalyzeRun
        {
            Id = runId,
            PromptId = promptId,
            Provider = "anthropic",
            ModelId = "claude-3",
            Input = "in",
            Output = "out",
            Temperature = 0.5m,
            MaxTokens = 512,
            TopP = 0.9m,
            PromptSnapshot = "snap",
            PromptSnapshotHash = "hash",
            Status = "Completed",
            ErrorMessage = "err",
            LatencyMs = 99,
            CreatedAt = DateTime.UtcNow
        };

        var result = await repo.CreateRunAsync(run, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.EntityId.Should().Be(runId);
    }

    [Fact]
    public async Task GetRunByIdAsync_ReturnsRun()
    {
        var id = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.AnalyzeGetRunById);
                cmd.GetParameterValues()["Id"].Should().Be(id);
                var t = new DataTable();
                t.Columns.Add("Id", typeof(Guid));
                t.Columns.Add("PromptId", typeof(Guid));
                t.Columns.Add("PromptTitle", typeof(string));
                t.Columns.Add("Provider", typeof(string));
                t.Columns.Add("ModelId", typeof(string));
                t.Columns.Add("Input", typeof(string));
                t.Columns.Add("Output", typeof(string));
                t.Columns.Add("Temperature", typeof(decimal));
                t.Columns.Add("MaxTokens", typeof(int));
                t.Columns.Add("TopP", typeof(decimal));
                t.Columns.Add("PromptSnapshot", typeof(string));
                t.Columns.Add("PromptSnapshotHash", typeof(string));
                t.Columns.Add("Status", typeof(string));
                t.Columns.Add("ErrorMessage", typeof(string));
                t.Columns.Add("LatencyMs", typeof(int));
                t.Columns.Add("CreatedAt", typeof(DateTime));
                t.Columns.Add("CompletedAt", typeof(DateTime));
                var now = DateTime.UtcNow;
                var row = t.NewRow();
                row["Id"] = id;
                row["PromptId"] = Guid.NewGuid();
                row["PromptTitle"] = DBNull.Value;
                row["Provider"] = "p";
                row["ModelId"] = DBNull.Value;
                row["Input"] = DBNull.Value;
                row["Output"] = "out";
                row["Temperature"] = DBNull.Value;
                row["MaxTokens"] = DBNull.Value;
                row["TopP"] = DBNull.Value;
                row["PromptSnapshot"] = DBNull.Value;
                row["PromptSnapshotHash"] = DBNull.Value;
                row["Status"] = "Completed";
                row["ErrorMessage"] = DBNull.Value;
                row["LatencyMs"] = 10;
                row["CreatedAt"] = now;
                row["CompletedAt"] = DBNull.Value;
                t.Rows.Add(row);
                return t;
            }
        };
        var repo = new AnalyzeRepository(new TestDbConnectionFactory(conn));

        var run = await repo.GetRunByIdAsync(id, CancellationToken.None);

        run.Should().NotBeNull();
        run!.Output.Should().Be("out");
    }

    [Fact]
    public async Task GetRunByIdAsync_WhenNoRow_ReturnsNull()
    {
        var id = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.AnalyzeGetRunById);
                cmd.GetParameterValues()["Id"].Should().Be(id);
                return new DataTable();
            }
        };
        var repo = new AnalyzeRepository(new TestDbConnectionFactory(conn));

        var run = await repo.GetRunByIdAsync(id, CancellationToken.None);

        run.Should().BeNull();
    }
}
