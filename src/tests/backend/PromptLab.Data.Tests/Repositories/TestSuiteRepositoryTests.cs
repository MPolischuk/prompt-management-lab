using System.Data;
using FluentAssertions;
using PromptLab.Data.Infrastructure;
using PromptLab.Data.Repositories;
using PromptLab.Data.Tests.Fakes;
using PromptLab.Data.Tests.Infrastructure;
using PromptLab.Entities.TestSuites;

namespace PromptLab.Data.Tests.Repositories;

public class TestSuiteRepositoryTests
{
    public TestSuiteRepositoryTests() => RepoDbTestInitializer.EnsureSqlServer();

    [Fact]
    public async Task GetByPromptIdAsync_PassesPromptId()
    {
        var promptId = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestSuiteGetByPromptId);
                cmd.GetParameterValues()["PromptId"].Should().Be(promptId);
                return new DataTable();
            }
        };
        var repo = new TestSuiteRepository(new TestDbConnectionFactory(conn));

        var list = await repo.GetByPromptIdAsync(promptId, CancellationToken.None);

        list.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsSuite()
    {
        var id = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TestSuiteGetById);
                cmd.GetParameterValues()["Id"].Should().Be(id);
                var t = new DataTable();
                t.Columns.Add("Id", typeof(Guid));
                t.Columns.Add("PromptId", typeof(Guid));
                t.Columns.Add("Name", typeof(string));
                t.Columns.Add("Description", typeof(string));
                t.Columns.Add("IsActive", typeof(bool));
                t.Columns.Add("CreatedAt", typeof(DateTime));
                t.Columns.Add("UpdatedAt", typeof(DateTime));
                var now = DateTime.UtcNow;
                var row = t.NewRow();
                row["Id"] = id;
                row["PromptId"] = Guid.NewGuid();
                row["Name"] = "S";
                row["Description"] = DBNull.Value;
                row["IsActive"] = true;
                row["CreatedAt"] = now;
                row["UpdatedAt"] = now;
                t.Rows.Add(row);
                return t;
            }
        };
        var repo = new TestSuiteRepository(new TestDbConnectionFactory(conn));

        var suite = await repo.GetByIdAsync(id, CancellationToken.None);

        suite.Should().NotBeNull();
        suite!.Name.Should().Be("S");
    }

    [Fact]
    public async Task CreateUpdateDelete_PropagateParameters()
    {
        var promptId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd => cmd.CommandText switch
            {
                StoredProcedures.TestSuiteCreate =>
                    DataTableTestHelpers.OperationResultTable(true, Guid.NewGuid()),
                StoredProcedures.TestSuiteUpdate =>
                    DataTableTestHelpers.OperationResultTable(true, id),
                StoredProcedures.TestSuiteDelete =>
                    DataTableTestHelpers.OperationResultTable(true),
                _ => throw new InvalidOperationException(cmd.CommandText)
            }
        };
        var repo = new TestSuiteRepository(new TestDbConnectionFactory(conn));

        var create = await repo.CreateAsync(new CreateTestSuiteRequest { PromptId = promptId, Name = "n", Description = "d" }, CancellationToken.None);
        create.Success.Should().BeTrue();

        var update = await repo.UpdateAsync(id, new UpdateTestSuiteRequest { Name = "n2", Description = null }, CancellationToken.None);
        update.Success.Should().BeTrue();

        var delete = await repo.DeleteAsync(id, CancellationToken.None);
        delete.Success.Should().BeTrue();
    }
}
