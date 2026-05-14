using System.Data;
using PromptLab.Data.Infrastructure;

namespace PromptLab.Data.Tests.Fakes;

internal sealed class TestDbConnectionFactory(TestDbConnection connection) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => connection;
}
