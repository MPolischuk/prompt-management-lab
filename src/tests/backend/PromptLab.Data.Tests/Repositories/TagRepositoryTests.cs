using System.Data;
using FluentAssertions;
using PromptLab.Data.Infrastructure;
using PromptLab.Data.Repositories;
using PromptLab.Data.Tests.Fakes;
using PromptLab.Data.Tests.Infrastructure;
using PromptLab.Entities.Tags;

namespace PromptLab.Data.Tests.Repositories;

public class TagRepositoryTests
{
    public TagRepositoryTests() => RepoDbTestInitializer.EnsureSqlServer();

    [Fact]
    public async Task GetAllAsync_UsesTagGetAll()
    {
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TagGetAll);
                cmd.CommandType.Should().Be(CommandType.StoredProcedure);
                var t = new DataTable();
                t.Columns.Add("Id", typeof(Guid));
                t.Columns.Add("Name", typeof(string));
                t.Columns.Add("Slug", typeof(string));
                t.Columns.Add("CreatedAt", typeof(DateTime));
                t.Columns.Add("UpdatedAt", typeof(DateTime));
                var id = Guid.NewGuid();
                var now = DateTime.UtcNow;
                var row = t.NewRow();
                row["Id"] = id;
                row["Name"] = "n";
                row["Slug"] = "s";
                row["CreatedAt"] = now;
                row["UpdatedAt"] = now;
                t.Rows.Add(row);
                return t;
            }
        };
        var repo = new TagRepository(new TestDbConnectionFactory(conn));

        var tags = await repo.GetAllAsync(CancellationToken.None);

        tags.Should().HaveCount(1);
        tags.First().Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SearchAsync_PassesQueryParameter()
    {
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TagSearch);
                cmd.GetParameterValues()["Query"].Should().Be("ab");
                return new DataTable();
            }
        };
        var repo = new TagRepository(new TestDbConnectionFactory(conn));

        var tags = await repo.SearchAsync("ab", CancellationToken.None);

        tags.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_PassesName()
    {
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.TagCreate);
                cmd.GetParameterValues()["Name"].Should().Be("tag1");
                return DataTableTestHelpers.OperationResultTable(true, Guid.NewGuid());
            }
        };
        var repo = new TagRepository(new TestDbConnectionFactory(conn));

        var result = await repo.CreateAsync(new CreateTagRequest { Name = "tag1" }, CancellationToken.None);

        result.Success.Should().BeTrue();
    }
}
