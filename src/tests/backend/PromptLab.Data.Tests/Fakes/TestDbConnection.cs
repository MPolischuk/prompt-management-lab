using System.Data;
using System.Data.Common;

namespace PromptLab.Data.Tests.Fakes;

/// <summary>
/// Conexión de prueba que delega la ejecución en un <see cref="DataTable"/> para materializar filas con RepoDb.
/// </summary>
internal sealed class TestDbConnection : DbConnection
{
    private ConnectionState _state = ConnectionState.Closed;

    public override string ConnectionString { get; set; } = "Test";

    public override string Database => "";

    public override string DataSource => "Test";

    public override string ServerVersion => "0.0";

    public override ConnectionState State => _state;

    /// <summary>Último comando ejecutado (texto, tipo y parámetros).</summary>
    public TestDbCommand? LastCommand { get; private set; }

    /// <summary>Devuelve el resultado para el próximo ExecuteReaderAsync.</summary>
    public Func<TestDbCommand, DataTable> ResultBuilder { get; set; } = _ => new DataTable();

    public override void ChangeDatabase(string databaseName) { }

    public override void Close() => _state = ConnectionState.Closed;

    public override void Open() => _state = ConnectionState.Open;

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
        throw new NotSupportedException();

    protected override DbCommand CreateDbCommand()
    {
        var cmd = new TestDbCommand(this);
        LastCommand = cmd;
        return cmd;
    }

    internal DataTable BuildResult(TestDbCommand command)
    {
        if (RepoDbIntrospectionResultBuilder.IsFieldDiscoveryQuery(command.CommandText))
        {
            return RepoDbIntrospectionResultBuilder.BuildForIntrospection(command);
        }

        return ResultBuilder(command);
    }
}
