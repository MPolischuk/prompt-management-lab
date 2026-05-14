using System.Data;
using FluentAssertions;
using PromptLab.Data.Infrastructure;
using PromptLab.Data.Repositories;
using PromptLab.Data.Tests.Fakes;
using PromptLab.Data.Tests.Infrastructure;
using PromptLab.Entities.Prompts;

namespace PromptLab.Data.Tests.Repositories;

public class PromptVersionRepositoryTests
{
    public PromptVersionRepositoryTests() => RepoDbTestInitializer.EnsureSqlServer();

    [Fact]
    public async Task GetByPromptIdAsync_UsesPromptVersionGetByPromptId()
    {
        var promptId = Guid.NewGuid();
        var conn = new TestDbConnection
        {
            ResultBuilder = cmd =>
            {
                cmd.CommandText.Should().Be(StoredProcedures.PromptVersionGetByPromptId);
                cmd.GetParameterValues()["PromptId"].Should().Be(promptId);
                var t = new DataTable();
                t.Columns.Add("Id", typeof(Guid));
                t.Columns.Add("PromptId", typeof(Guid));
                t.Columns.Add("Content", typeof(string));
                t.Columns.Add("Version", typeof(int));
                t.Columns.Add("CreatedAt", typeof(DateTime));
                var row = t.NewRow();
                row["Id"] = Guid.NewGuid();
                row["PromptId"] = promptId;
                row["Content"] = "v1";
                row["Version"] = 1;
                row["CreatedAt"] = DateTime.UtcNow;
                t.Rows.Add(row);
                return t;
            }
        };
        var repo = new PromptVersionRepository(new TestDbConnectionFactory(conn));

        var list = await repo.GetByPromptIdAsync(promptId, CancellationToken.None);

        list.Should().HaveCount(1);
        list[0].Content.Should().Be("v1");
    }
}
