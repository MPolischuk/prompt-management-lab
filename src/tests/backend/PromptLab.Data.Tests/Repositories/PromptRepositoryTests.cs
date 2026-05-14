using System.Data;
using FluentAssertions;
using PromptLab.Data.Infrastructure;
using PromptLab.Data.Repositories;
using PromptLab.Data.Tests.Fakes;
using PromptLab.Data.Tests.Infrastructure;
using PromptLab.Entities.Common;
using PromptLab.Entities.Prompts;

namespace PromptLab.Data.Tests.Repositories;

public class PromptRepositoryTests
{
    public PromptRepositoryTests() => RepoDbTestInitializer.EnsureSqlServer();

    [Fact]
    public async Task CreateAsync_ExecutesPromptCreateWithParameters()
    {
        var entityId = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.PromptCreate);
                cmd.CommandType.Should().Be(CommandType.StoredProcedure);
                var p = cmd.GetParameterValues();
                p["Title"].Should().Be("My title");
                p["Content"].Should().Be("Body");
                return DataTableTestHelpers.OperationResultTable(true, entityId);
            }
        };
        var repo = new PromptRepository(new TestDbConnectionFactory(conn));
        var request = new UpsertPromptRequest
        {
            Title = "My title",
            Content = "Body",
            Description = "d",
            Category = "c",
            Language = "es",
            ModelHint = "gpt",
            TargetModelId = "m1",
            Temperature = 0.7m,
            MaxTokens = 100,
            TopP = 0.9m
        };

        var result = await repo.CreateAsync(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.EntityId.Should().Be(entityId);
    }

    [Fact]
    public async Task UpdateAsync_ExecutesPromptUpdateWithIdAndIsActive()
    {
        var id = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.PromptUpdate);
                var p = cmd.GetParameterValues();
                p["Id"].Should().Be(id);
                p["IsActive"].Should().Be(false);
                return DataTableTestHelpers.OperationResultTable(true, id);
            }
        };
        var repo = new PromptRepository(new TestDbConnectionFactory(conn));
        var request = new UpsertPromptRequest { Title = "t", Content = "c", IsActive = false };

        var result = await repo.UpdateAsync(id, request, CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ExecutesPromptDelete()
    {
        var id = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.PromptDelete);
                cmd.GetParameterValues()["Id"].Should().Be(id);
                return DataTableTestHelpers.OperationResultTable(true);
            }
        };
        var repo = new PromptRepository(new TestDbConnectionFactory(conn));

        var result = await repo.DeleteAsync(id, CancellationToken.None);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_WhenRowExists_MapsPrompt()
    {
        var id = Guid.NewGuid();
        var created = DateTime.UtcNow;
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.PromptGetById);
                cmd.GetParameterValues()["Id"].Should().Be(id);
                var t = new DataTable();
                t.Columns.Add("Id", typeof(Guid));
                t.Columns.Add("Title", typeof(string));
                t.Columns.Add("Description", typeof(string));
                t.Columns.Add("Content", typeof(string));
                t.Columns.Add("Category", typeof(string));
                t.Columns.Add("Language", typeof(string));
                t.Columns.Add("ModelHint", typeof(string));
                t.Columns.Add("TargetModelId", typeof(string));
                t.Columns.Add("Temperature", typeof(decimal));
                t.Columns.Add("MaxTokens", typeof(int));
                t.Columns.Add("TopP", typeof(decimal));
                t.Columns.Add("Version", typeof(int));
                t.Columns.Add("IsActive", typeof(bool));
                t.Columns.Add("CreatedAt", typeof(DateTime));
                t.Columns.Add("UpdatedAt", typeof(DateTime));
                t.Columns.Add("TagsJson", typeof(string));
                var row = t.NewRow();
                row["Id"] = id;
                row["Title"] = "T";
                row["Description"] = DBNull.Value;
                row["Content"] = "C";
                row["Category"] = DBNull.Value;
                row["Language"] = DBNull.Value;
                row["ModelHint"] = DBNull.Value;
                row["TargetModelId"] = DBNull.Value;
                row["Temperature"] = DBNull.Value;
                row["MaxTokens"] = DBNull.Value;
                row["TopP"] = DBNull.Value;
                row["Version"] = 1;
                row["IsActive"] = true;
                row["CreatedAt"] = created;
                row["UpdatedAt"] = created;
                row["TagsJson"] = DBNull.Value;
                t.Rows.Add(row);
                return t;
            }
        };
        var repo = new PromptRepository(new TestDbConnectionFactory(conn));

        var prompt = await repo.GetByIdAsync(id, CancellationToken.None);

        prompt.Should().NotBeNull();
        prompt!.Id.Should().Be(id);
        prompt.Title.Should().Be("T");
        prompt.Content.Should().Be("C");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNoRow_ReturnsNull()
    {
        var conn = new TestDbConnection
        {
            ResultBuilder = _ =>
            {
                var t = new DataTable();
                t.Columns.Add("Id", typeof(Guid));
                return t;
            }
        };
        var repo = new PromptRepository(new TestDbConnectionFactory(conn));

        var prompt = await repo.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        prompt.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_ReturnsPagedResponseWithTotalRows()
    {
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.PromptSearch);
                var p = cmd.GetParameterValues();
                p["PageNumber"].Should().Be(2);
                p["PageSize"].Should().Be(5);
                var t = new DataTable();
                t.Columns.Add("Id", typeof(Guid));
                t.Columns.Add("Title", typeof(string));
                t.Columns.Add("Description", typeof(string));
                t.Columns.Add("Content", typeof(string));
                t.Columns.Add("Category", typeof(string));
                t.Columns.Add("Language", typeof(string));
                t.Columns.Add("ModelHint", typeof(string));
                t.Columns.Add("TargetModelId", typeof(string));
                t.Columns.Add("Temperature", typeof(decimal));
                t.Columns.Add("MaxTokens", typeof(int));
                t.Columns.Add("TopP", typeof(decimal));
                t.Columns.Add("Version", typeof(int));
                t.Columns.Add("IsActive", typeof(bool));
                t.Columns.Add("CreatedAt", typeof(DateTime));
                t.Columns.Add("UpdatedAt", typeof(DateTime));
                t.Columns.Add("TagsJson", typeof(string));
                t.Columns.Add("TotalRows", typeof(int));
                var row = t.NewRow();
                row["Id"] = Guid.NewGuid();
                row["Title"] = "A";
                row["Description"] = DBNull.Value;
                row["Content"] = "x";
                row["Category"] = DBNull.Value;
                row["Language"] = DBNull.Value;
                row["ModelHint"] = DBNull.Value;
                row["TargetModelId"] = DBNull.Value;
                row["Temperature"] = DBNull.Value;
                row["MaxTokens"] = DBNull.Value;
                row["TopP"] = DBNull.Value;
                row["Version"] = 1;
                row["IsActive"] = true;
                row["CreatedAt"] = DateTime.UtcNow;
                row["UpdatedAt"] = DateTime.UtcNow;
                row["TagsJson"] = DBNull.Value;
                row["TotalRows"] = 42;
                t.Rows.Add(row);
                return t;
            }
        };
        var repo = new PromptRepository(new TestDbConnectionFactory(conn));
        var request = new PromptSearchRequest { PageNumber = 2, PageSize = 5 };

        var page = await repo.SearchAsync(request, CancellationToken.None);

        page.TotalRows.Should().Be(42);
        page.Items.Should().HaveCount(1);
        page.PageNumber.Should().Be(2);
        page.PageSize.Should().Be(5);
    }

    [Fact]
    public async Task SetTagsAsync_JoinsTagIdsAsCsv()
    {
        var promptId = Guid.NewGuid();
        var t1 = Guid.NewGuid();
        var t2 = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.PromptSetTags);
                var p = cmd.GetParameterValues();
                p["PromptId"].Should().Be(promptId);
                p["TagIds"].Should().Be($"{t1},{t2}");
                return DataTableTestHelpers.OperationResultTable(true);
            }
        };
        var repo = new PromptRepository(new TestDbConnectionFactory(conn));

        var result = await repo.SetTagsAsync(promptId, [t1, t2], CancellationToken.None);

        result.Success.Should().BeTrue();
    }
}
