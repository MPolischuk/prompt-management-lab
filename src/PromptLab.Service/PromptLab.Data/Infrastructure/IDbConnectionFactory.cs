using System.Data;

namespace PromptLab.Data.Infrastructure;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
