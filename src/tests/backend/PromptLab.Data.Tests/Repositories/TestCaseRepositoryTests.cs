using System.Data;
using FluentAssertions;
using PromptLab.Data.Infrastructure;
using PromptLab.Data.Repositories;
using PromptLab.Data.Tests.Fakes;
using PromptLab.Data.Tests.Infrastructure;
using PromptLab.Entities.TestCases;

namespace PromptLab.Data.Tests.Repositories;

public class TestCaseRepositoryTests
{
    public TestCaseRepositoryTests() => RepoDbTestInitializer.EnsureSqlServer();

    [Fact]
    public async Task GetBySuiteIdAsync_PassesSuiteId()
    {
        var suiteId = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestCaseGetBySuiteId);
                cmd.GetParameterValues()["SuiteId"].Should().Be(suiteId);
                return new DataTable();
            }
        };
        var repo = new TestCaseRepository(new TestDbConnectionFactory(conn));

        var list = await repo.GetBySuiteIdAsync(suiteId, CancellationToken.None);

        list.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBySuiteIdAsync_WithRows_MapsTestCases()
    {
        var suiteId = Guid.NewGuid();
        var caseId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestCaseGetBySuiteId);
                cmd.GetParameterValues()["SuiteId"].Should().Be(suiteId);
                var t = new DataTable();
                t.Columns.Add("Id", typeof(Guid));
                t.Columns.Add("SuiteId", typeof(Guid));
                t.Columns.Add("Name", typeof(string));
                t.Columns.Add("InputVariables", typeof(string));
                t.Columns.Add("ExpectedOutput", typeof(string));
                t.Columns.Add("IsActive", typeof(bool));
                t.Columns.Add("CreatedAt", typeof(DateTime));
                t.Columns.Add("UpdatedAt", typeof(DateTime));
                var row = t.NewRow();
                row["Id"] = caseId;
                row["SuiteId"] = suiteId;
                row["Name"] = "Case1";
                row["InputVariables"] = "{}";
                row["ExpectedOutput"] = "exp";
                row["IsActive"] = true;
                row["CreatedAt"] = now;
                row["UpdatedAt"] = now;
                t.Rows.Add(row);
                return t;
            }
        };
        var repo = new TestCaseRepository(new TestDbConnectionFactory(conn));

        var list = await repo.GetBySuiteIdAsync(suiteId, CancellationToken.None);

        list.Should().HaveCount(1);
        list[0].Id.Should().Be(caseId);
        list[0].Name.Should().Be("Case1");
        list[0].InputVariables.Should().Be("{}");
        list[0].ExpectedOutput.Should().Be("exp");
    }

    [Fact]
    public async Task CreateAsync_ValidatesParameters()
    {
        var suiteId = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestCaseCreate);
                var p = cmd.GetParameterValues();
                p["SuiteId"].Should().Be(suiteId);
                p["Name"].Should().Be("c");
                p["InputVariables"].Should().Be("{}");
                p["ExpectedOutput"].Should().Be("x");
                return DataTableTestHelpers.OperationResultTable(true, Guid.NewGuid());
            }
        };
        var repo = new TestCaseRepository(new TestDbConnectionFactory(conn));

        var result = await repo.CreateAsync(
            new CreateTestCaseRequest { SuiteId = suiteId, Name = "c", InputVariables = "{}", ExpectedOutput = "x" },
            CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_ValidatesParameters()
    {
        var id = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestCaseUpdate);
                var p = cmd.GetParameterValues();
                p["Id"].Should().Be(id);
                p["Name"].Should().Be("c2");
                p["InputVariables"].Should().Be("{}");
                p["ExpectedOutput"].Should().BeNull();
                return DataTableTestHelpers.OperationResultTable(true, id);
            }
        };
        var repo = new TestCaseRepository(new TestDbConnectionFactory(conn));

        var result = await repo.UpdateAsync(id, new UpdateTestCaseRequest { Name = "c2", InputVariables = "{}", ExpectedOutput = null }, CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ValidatesParameters()
    {
        var id = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestCaseDelete);
                cmd.GetParameterValues()["Id"].Should().Be(id);
                return DataTableTestHelpers.OperationResultTable(true);
            }
        };
        var repo = new TestCaseRepository(new TestDbConnectionFactory(conn));

        var del = await repo.DeleteAsync(id, CancellationToken.None);

        del.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateUpdateDelete_UsesStoredProcedures()
    {
        var suiteId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd => cmd.CommandText switch
            {
                StoredProcedures.TestCaseCreate => DataTableTestHelpers.OperationResultTable(true, Guid.NewGuid()),
                StoredProcedures.TestCaseUpdate => DataTableTestHelpers.OperationResultTable(true, id),
                StoredProcedures.TestCaseDelete => DataTableTestHelpers.OperationResultTable(true),
                _ => throw new InvalidOperationException(cmd.CommandText)
            }
        };
        var repo = new TestCaseRepository(new TestDbConnectionFactory(conn));

        await repo.CreateAsync(
            new CreateTestCaseRequest { SuiteId = suiteId, Name = "c", InputVariables = "{}", ExpectedOutput = "x" },
            CancellationToken.None);
        await repo.UpdateAsync(id, new UpdateTestCaseRequest { Name = "c2", InputVariables = "{}", ExpectedOutput = null }, CancellationToken.None);
        var del = await repo.DeleteAsync(id, CancellationToken.None);

        del.Success.Should().BeTrue();
    }
}
