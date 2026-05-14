using Microsoft.Data.SqlClient;
using PromptLab.Data.Tests.Fakes;
using RepoDb;

namespace PromptLab.Data.Tests.Infrastructure;

/// <summary>
/// RepoDb requiere configuración global antes de ejecutar consultas.
/// </summary>
public static class RepoDbTestInitializer
{
    private static readonly object Gate = new();
    private static bool _initialized;

    public static void EnsureSqlServer()
    {
        if (_initialized)
        {
            return;
        }

        lock (Gate)
        {
            if (_initialized)
            {
                return;
            }

            GlobalConfiguration.Setup().UseSqlServer();
            // Permite usar <see cref="TestDbConnection"/> con el mismo helper y settings que SqlConnection.
            DbHelperMapper.Add<TestDbConnection>(DbHelperMapper.Get<SqlConnection>(), true);
            DbSettingMapper.Add<TestDbConnection>(DbSettingMapper.Get<SqlConnection>(), true);
            _initialized = true;
        }
    }
}
