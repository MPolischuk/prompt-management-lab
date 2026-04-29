using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using PromptLab.Data.Configuration;

namespace PromptLab.Data.Infrastructure;

public class SqlConnectionFactory(IOptions<SqlOptions> options) : IDbConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(options.Value.ConnectionString);
    }
}
