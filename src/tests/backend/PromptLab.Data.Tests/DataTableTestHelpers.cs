using System.Data;
using PromptLab.Entities.Common;

namespace PromptLab.Data.Tests;

internal static class DataTableTestHelpers
{
    public static DataTable OperationResultTable(bool success, Guid? entityId = null, string? message = null, OperationErrorCode errorCode = OperationErrorCode.None)
    {
        var t = new DataTable();
        t.Columns.Add("Success", typeof(bool));
        t.Columns.Add("EntityId", typeof(Guid));
        t.Columns.Add("Message", typeof(string));
        t.Columns.Add("ErrorCode", typeof(int));
        var row = t.NewRow();
        row["Success"] = success;
        row["EntityId"] = entityId.HasValue ? entityId.Value : DBNull.Value;
        row["Message"] = message is null ? DBNull.Value : message;
        row["ErrorCode"] = (int)errorCode;
        t.Rows.Add(row);
        return t;
    }
}
