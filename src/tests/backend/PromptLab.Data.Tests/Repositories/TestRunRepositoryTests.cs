using System.Data;
using FluentAssertions;
using PromptLab.Data.Infrastructure;
using PromptLab.Data.Repositories;
using PromptLab.Data.Tests.Fakes;
using PromptLab.Data.Tests.Infrastructure;
using PromptLab.Entities.TestResults;
using PromptLab.Entities.TestRuns;

namespace PromptLab.Data.Tests.Repositories;

public class TestRunRepositoryTests
{
    public TestRunRepositoryTests() => RepoDbTestInitializer.EnsureSqlServer();

    [Fact]
    public async Task GetAllAsync_UsesTestRunGetAll()
    {
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestRunGetAll);
                return new DataTable();
            }
        };
        var repo = new TestRunRepository(new TestDbConnectionFactory(conn));

        var list = await repo.GetAllAsync(CancellationToken.None);

        list.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBySuiteIdAsync_PassesSuiteId()
    {
        var suiteId = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestRunGetBySuiteId);
                cmd.GetParameterValues()["SuiteId"].Should().Be(suiteId);
                return new DataTable();
            }
        };
        var repo = new TestRunRepository(new TestDbConnectionFactory(conn));

        var list = await repo.GetBySuiteIdAsync(suiteId, CancellationToken.None);

        list.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsRun()
    {
        var id = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestRunGetById);
                cmd.GetParameterValues()["Id"].Should().Be(id);
                var t = new DataTable();
                t.Columns.Add("Id", typeof(Guid));
                t.Columns.Add("SuiteId", typeof(Guid));
                t.Columns.Add("PromptId", typeof(Guid));
                t.Columns.Add("PromptVersion", typeof(int));
                t.Columns.Add("Model", typeof(string));
                t.Columns.Add("Temperature", typeof(decimal));
                t.Columns.Add("Status", typeof(string));
                t.Columns.Add("StartedAt", typeof(DateTime));
                t.Columns.Add("CompletedAt", typeof(DateTime));
                t.Columns.Add("CreatedAt", typeof(DateTime));
                t.Columns.Add("PromptTitle", typeof(string));
                t.Columns.Add("SuiteName", typeof(string));
                var now = DateTime.UtcNow;
                var row = t.NewRow();
                row["Id"] = id;
                row["SuiteId"] = Guid.NewGuid();
                row["PromptId"] = Guid.NewGuid();
                row["PromptVersion"] = 1;
                row["Model"] = "m";
                row["Temperature"] = 0.5m;
                row["Status"] = "Pending";
                row["StartedAt"] = DBNull.Value;
                row["CompletedAt"] = DBNull.Value;
                row["CreatedAt"] = now;
                row["PromptTitle"] = DBNull.Value;
                row["SuiteName"] = DBNull.Value;
                t.Rows.Add(row);
                return t;
            }
        };
        var repo = new TestRunRepository(new TestDbConnectionFactory(conn));

        var run = await repo.GetByIdAsync(id, CancellationToken.None);

        run.Should().NotBeNull();
        run!.Model.Should().Be("m");
    }

    [Fact]
    public async Task CreateUpdateCreateResultGetResults_Flow()
    {
        var id = Guid.NewGuid();
        var runId = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd => cmd.CommandText switch
            {
                StoredProcedures.TestRunCreate => DataTableTestHelpers.OperationResultTable(true, runId),
                StoredProcedures.TestRunUpdate => DataTableTestHelpers.OperationResultTable(true, id),
                StoredProcedures.TestResultCreate => DataTableTestHelpers.OperationResultTable(true, Guid.NewGuid()),
                StoredProcedures.TestResultGetByRunId => new DataTable(),
                _ => throw new InvalidOperationException(cmd.CommandText)
            }
        };
        var repo = new TestRunRepository(new TestDbConnectionFactory(conn));

        var create = await repo.CreateAsync(
            new CreateTestRunRequest
            {
                SuiteId = Guid.NewGuid(),
                PromptId = Guid.NewGuid(),
                PromptVersion = 1,
                Model = "m",
                Temperature = 0.1m,
                Status = "Pending"
            },
            CancellationToken.None);
        create.Success.Should().BeTrue();

        var update = await repo.UpdateAsync(
            id,
            new UpdateTestRunRequest { Status = "Done", StartedAt = DateTime.UtcNow, CompletedAt = DateTime.UtcNow },
            CancellationToken.None);
        update.Success.Should().BeTrue();

        var res = await repo.CreateResultAsync(
            new CreateTestResultRequest
            {
                RunId = runId,
                CaseId = Guid.NewGuid(),
                ActualOutput = "a",
                Passed = true,
                Score = 1m,
                LatencyMs = 5,
                Error = null
            },
            CancellationToken.None);
        res.Success.Should().BeTrue();

        var results = await repo.GetResultsByRunIdAsync(runId, CancellationToken.None);
        results.Should().BeEmpty();
    }
}
