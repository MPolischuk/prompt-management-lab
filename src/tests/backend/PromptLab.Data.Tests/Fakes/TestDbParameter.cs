using System.Data;
using System.Data.Common;

namespace PromptLab.Data.Tests.Fakes;

internal sealed class TestDbParameter : DbParameter
{
    public override int Size { get; set; }

    public override DbType DbType { get; set; }
    public override ParameterDirection Direction { get; set; } = ParameterDirection.Input;
    public override bool IsNullable { get; set; }
    public override string ParameterName { get; set; } = string.Empty;
    public override string SourceColumn { get; set; } = string.Empty;
    public override bool SourceColumnNullMapping { get; set; }
    public override object? Value { get; set; }
    public override void ResetDbType() { }
}
