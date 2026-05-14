using System.Data;
using System.Data.Common;

namespace PromptLab.Data.Tests.Fakes;

internal sealed class TestDbCommand : DbCommand
{
    private TestDbConnection _connection;
    private readonly TestParameterCollection _parameters = new();

    public TestDbCommand(TestDbConnection connection) => _connection = connection;

    public override string CommandText { get; set; } = "";

    public override int CommandTimeout { get; set; } = 30;

    public override CommandType CommandType { get; set; } = CommandType.Text;

    public override bool DesignTimeVisible { get; set; }

    public override UpdateRowSource UpdatedRowSource { get; set; }

    protected override DbConnection? DbConnection
    {
        get => _connection;
        set => _connection = (TestDbConnection)(value ?? throw new ArgumentNullException(nameof(value)));
    }

    protected override DbParameterCollection DbParameterCollection => _parameters;

    protected override DbTransaction? DbTransaction { get; set; }

    public override void Cancel() { }

    public override int ExecuteNonQuery() => throw new NotSupportedException();

    public override object? ExecuteScalar() => throw new NotSupportedException();

    public override void Prepare() { }

    protected override DbParameter CreateDbParameter() => new TestDbParameter();

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
        throw new NotSupportedException("Use async");

    protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
    {
        await Task.Yield();
        var table = _connection.BuildResult(this);
        return table.CreateDataReader();
    }

    public IReadOnlyDictionary<string, object?> GetParameterValues()
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (DbParameter p in _parameters)
        {
            var name = p.ParameterName.StartsWith('@') ? p.ParameterName[1..] : p.ParameterName;
            dict[name] = p.Value is DBNull ? null : p.Value;
        }

        return dict;
    }
}
