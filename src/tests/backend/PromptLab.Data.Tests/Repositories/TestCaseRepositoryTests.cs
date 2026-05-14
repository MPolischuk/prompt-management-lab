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
