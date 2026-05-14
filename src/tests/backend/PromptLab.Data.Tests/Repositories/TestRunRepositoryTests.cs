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
    public async Task GetByIdAsync_WhenNoRow_ReturnsNull()
    {
        var id = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestRunGetById);
                cmd.GetParameterValues()["Id"].Should().Be(id);
                return new DataTable();
            }
        };
        var repo = new TestRunRepository(new TestDbConnectionFactory(conn));

        var run = await repo.GetByIdAsync(id, CancellationToken.None);

        run.Should().BeNull();
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

    [Fact]
    public async Task GetResultsByRunIdAsync_WithRows_MapsResults()
    {
        var runId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestResultGetByRunId);
                cmd.GetParameterValues()["RunId"].Should().Be(runId);
                var t = new DataTable();
                t.Columns.Add("Id", typeof(Guid));
                t.Columns.Add("RunId", typeof(Guid));
                t.Columns.Add("CaseId", typeof(Guid));
                t.Columns.Add("ActualOutput", typeof(string));
                t.Columns.Add("Passed", typeof(bool));
                t.Columns.Add("Score", typeof(decimal));
                t.Columns.Add("LatencyMs", typeof(int));
                t.Columns.Add("Error", typeof(string));
                t.Columns.Add("CreatedAt", typeof(DateTime));
                t.Columns.Add("CaseName", typeof(string));
                t.Columns.Add("InputVariables", typeof(string));
                t.Columns.Add("ExpectedOutput", typeof(string));
                var row = t.NewRow();
                row["Id"] = resultId;
                row["RunId"] = runId;
                row["CaseId"] = caseId;
                row["ActualOutput"] = "out";
                row["Passed"] = true;
                row["Score"] = 0.9m;
                row["LatencyMs"] = 12;
                row["Error"] = DBNull.Value;
                row["CreatedAt"] = now;
                row["CaseName"] = "cn";
                row["InputVariables"] = "{}";
                row["ExpectedOutput"] = "e";
                t.Rows.Add(row);
                return t;
            }
        };
        var repo = new TestRunRepository(new TestDbConnectionFactory(conn));

        var results = await repo.GetResultsByRunIdAsync(runId, CancellationToken.None);

        results.Should().HaveCount(1);
        results[0].Id.Should().Be(resultId);
        results[0].ActualOutput.Should().Be("out");
        results[0].Passed.Should().BeTrue();
        results[0].Score.Should().Be(0.9m);
        results[0].CaseName.Should().Be("cn");
    }

    [Fact]
    public async Task CreateAsync_ValidatesParameters()
    {
        var suiteId = Guid.NewGuid();
        var promptId = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestRunCreate);
                var p = cmd.GetParameterValues();
                p["SuiteId"].Should().Be(suiteId);
                p["PromptId"].Should().Be(promptId);
                p["PromptVersion"].Should().Be(2);
                p["Model"].Should().Be("m");
                p["Temperature"].Should().Be(0.3m);
                p["Status"].Should().Be("Pending");
                return DataTableTestHelpers.OperationResultTable(true, Guid.NewGuid());
            }
        };
        var repo = new TestRunRepository(new TestDbConnectionFactory(conn));

        var result = await repo.CreateAsync(
            new CreateTestRunRequest
            {
                SuiteId = suiteId,
                PromptId = promptId,
                PromptVersion = 2,
                Model = "m",
                Temperature = 0.3m,
                Status = "Pending"
            },
            CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_ValidatesParameters()
    {
        var id = Guid.NewGuid();
        var started = DateTime.UtcNow;
        var completed = DateTime.UtcNow.AddMinutes(1);
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestRunUpdate);
                var p = cmd.GetParameterValues();
                p["Id"].Should().Be(id);
                p["Status"].Should().Be("Done");
                p["StartedAt"].Should().Be(started);
                p["CompletedAt"].Should().Be(completed);
                return DataTableTestHelpers.OperationResultTable(true, id);
            }
        };
        var repo = new TestRunRepository(new TestDbConnectionFactory(conn));

        var result = await repo.UpdateAsync(
            id,
            new UpdateTestRunRequest { Status = "Done", StartedAt = started, CompletedAt = completed },
            CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateResultAsync_ValidatesParameters()
    {
        var runId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestResultCreate);
                var p = cmd.GetParameterValues();
                p["RunId"].Should().Be(runId);
                p["CaseId"].Should().Be(caseId);
                p["ActualOutput"].Should().Be("a");
                p["Passed"].Should().Be(true);
                p["Score"].Should().Be(1m);
                p["LatencyMs"].Should().Be(5);
                p["Error"].Should().BeNull();
                return DataTableTestHelpers.OperationResultTable(true, Guid.NewGuid());
            }
        };
        var repo = new TestRunRepository(new TestDbConnectionFactory(conn));

        var res = await repo.CreateResultAsync(
            new CreateTestResultRequest
            {
                RunId = runId,
                CaseId = caseId,
                ActualOutput = "a",
                Passed = true,
                Score = 1m,
                LatencyMs = 5,
                Error = null
            },
            CancellationToken.None);

        res.Success.Should().BeTrue();
    }
}
