using System.Data;

namespace PromptLab.Data.Tests.Fakes;

/// <summary>
/// RepoDb ejecuta una consulta a INFORMATION_SCHEMA antes de mapear resultados de SP.
/// Devolvemos metadatos de columnas coherentes con los tipos materializados en los repositorios.
/// </summary>
internal static class RepoDbIntrospectionResultBuilder
{
    public static bool IsFieldDiscoveryQuery(string commandText) =>
        commandText.Contains("INFORMATION_SCHEMA.COLUMNS", StringComparison.OrdinalIgnoreCase);

    public static DataTable BuildForIntrospection(TestDbCommand command)
    {
        var p = command.GetParameterValues();
        var tableName = p.TryGetValue("TableName", out var tn) ? tn?.ToString() ?? string.Empty : string.Empty;

        return tableName switch
        {
            var t when t.Contains("Prompt_Create", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("Prompt_Update", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("Prompt_Delete", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("Prompt_SetTags", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("Prompt_GetById", StringComparison.OrdinalIgnoreCase) => SchemaPromptDetailRow(),
            var t when t.Contains("Prompt_Search", StringComparison.OrdinalIgnoreCase) => SchemaPromptSearchRow(),
            var t when t.Contains("Tag_Create", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("Tag_GetAll", StringComparison.OrdinalIgnoreCase) => SchemaTag(),
            var t when t.Contains("Tag_Search", StringComparison.OrdinalIgnoreCase) => SchemaTag(),
            var t when t.Contains("Analyze_CreateRun", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("Analyze_GetRunById", StringComparison.OrdinalIgnoreCase) => SchemaAnalyzeRun(),
            var t when t.Contains("PromptVersion_GetByPromptId", StringComparison.OrdinalIgnoreCase) => SchemaPromptVersion(),
            var t when t.Contains("TestSuite_Create", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("TestSuite_Update", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("TestSuite_Delete", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("TestSuite_GetById", StringComparison.OrdinalIgnoreCase) => SchemaTestSuite(),
            var t when t.Contains("TestSuite_GetByPromptId", StringComparison.OrdinalIgnoreCase) => SchemaTestSuite(),
            var t when t.Contains("TestCase_Create", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("TestCase_Update", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("TestCase_Delete", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("TestCase_GetBySuiteId", StringComparison.OrdinalIgnoreCase) => SchemaTestCase(),
            var t when t.Contains("TestRun_Create", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("TestRun_Update", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("TestRun_GetById", StringComparison.OrdinalIgnoreCase) => SchemaTestRun(),
            var t when t.Contains("TestRun_GetBySuiteId", StringComparison.OrdinalIgnoreCase) => SchemaTestRun(),
            var t when t.Contains("TestRun_GetAll", StringComparison.OrdinalIgnoreCase) => SchemaTestRun(),
            var t when t.Contains("TestResult_Create", StringComparison.OrdinalIgnoreCase) => SchemaOperationResult(),
            var t when t.Contains("TestResult_GetByRunId", StringComparison.OrdinalIgnoreCase) => SchemaTestResult(),
            _ => SchemaOperationResult()
        };
    }

    private static DataTable CreateSchemaTable()
    {
        var t = new DataTable();
        t.Columns.Add("ColumnName", typeof(string));
        t.Columns.Add("IsPrimary", typeof(bool));
        t.Columns.Add("IsIdentity", typeof(bool));
        t.Columns.Add("IsNullable", typeof(bool));
        t.Columns.Add("DataType", typeof(string));
        t.Columns.Add("Size", typeof(int));
        t.Columns.Add("Precision", typeof(byte));
        t.Columns.Add("Scale", typeof(byte));
        t.Columns.Add("DefaultValue", typeof(bool));
        return t;
    }

    private static void AddRow(DataTable t, string column, string dataType, int size, bool nullable, bool isPrimary = false)
    {
        var row = t.NewRow();
        row["ColumnName"] = column;
        row["IsPrimary"] = isPrimary;
        row["IsIdentity"] = false;
        row["IsNullable"] = nullable;
        row["DataType"] = dataType;
        row["Size"] = size;
        row["Precision"] = (byte)1;
        row["Scale"] = (byte)1;
        row["DefaultValue"] = false;
        t.Rows.Add(row);
    }

    private static DataTable SchemaOperationResult()
    {
        var t = CreateSchemaTable();
        AddRow(t, "Success", "bit", 1, false);
        AddRow(t, "EntityId", "uniqueidentifier", 16, true);
        AddRow(t, "Message", "nvarchar", 4000, true);
        AddRow(t, "ErrorCode", "int", 4, false);
        return t;
    }

    private static DataTable SchemaPromptDetailRow()
    {
        var t = CreateSchemaTable();
        AddRow(t, "Id", "uniqueidentifier", 16, false, true);
        AddRow(t, "Title", "nvarchar", 200, false);
        AddRow(t, "Description", "nvarchar", 4000, true);
        AddRow(t, "Content", "nvarchar", -1, true);
        AddRow(t, "Category", "nvarchar", 200, true);
        AddRow(t, "Language", "nvarchar", 32, true);
        AddRow(t, "ModelHint", "nvarchar", 200, true);
        AddRow(t, "TargetModelId", "nvarchar", 200, true);
        AddRow(t, "Temperature", "decimal", 9, true);
        AddRow(t, "MaxTokens", "int", 4, true);
        AddRow(t, "TopP", "decimal", 9, true);
        AddRow(t, "Version", "int", 4, false);
        AddRow(t, "IsActive", "bit", 1, false);
        AddRow(t, "CreatedAt", "datetime", 8, false);
        AddRow(t, "UpdatedAt", "datetime", 8, false);
        AddRow(t, "TagsJson", "nvarchar", -1, true);
        return t;
    }

    private static DataTable SchemaPromptSearchRow()
    {
        var t = SchemaPromptDetailRow();
        AddRow(t, "TotalRows", "int", 4, false);
        return t;
    }

    private static DataTable SchemaTag()
    {
        var t = CreateSchemaTable();
        AddRow(t, "Id", "uniqueidentifier", 16, false, true);
        AddRow(t, "Name", "nvarchar", 200, false);
        AddRow(t, "Slug", "nvarchar", 200, false);
        AddRow(t, "CreatedAt", "datetime", 8, false);
        AddRow(t, "UpdatedAt", "datetime", 8, false);
        return t;
    }

    private static DataTable SchemaAnalyzeRun()
    {
        var t = CreateSchemaTable();
        AddRow(t, "Id", "uniqueidentifier", 16, false, true);
        AddRow(t, "PromptId", "uniqueidentifier", 16, false);
        AddRow(t, "PromptTitle", "nvarchar", 500, true);
        AddRow(t, "Provider", "nvarchar", 100, false);
        AddRow(t, "ModelId", "nvarchar", 200, true);
        AddRow(t, "Input", "nvarchar", -1, true);
        AddRow(t, "Output", "nvarchar", -1, true);
        AddRow(t, "Temperature", "decimal", 9, true);
        AddRow(t, "MaxTokens", "int", 4, true);
        AddRow(t, "TopP", "decimal", 9, true);
        AddRow(t, "PromptSnapshot", "nvarchar", -1, true);
        AddRow(t, "PromptSnapshotHash", "nvarchar", 200, true);
        AddRow(t, "Status", "nvarchar", 50, false);
        AddRow(t, "ErrorMessage", "nvarchar", 4000, true);
        AddRow(t, "LatencyMs", "int", 4, true);
        AddRow(t, "CreatedAt", "datetime", 8, false);
        AddRow(t, "CompletedAt", "datetime", 8, true);
        return t;
    }

    private static DataTable SchemaPromptVersion()
    {
        var t = CreateSchemaTable();
        AddRow(t, "Id", "uniqueidentifier", 16, false, true);
        AddRow(t, "PromptId", "uniqueidentifier", 16, false);
        AddRow(t, "Content", "nvarchar", -1, false);
        AddRow(t, "Version", "int", 4, false);
        AddRow(t, "CreatedAt", "datetime", 8, false);
        return t;
    }

    private static DataTable SchemaTestSuite()
    {
        var t = CreateSchemaTable();
        AddRow(t, "Id", "uniqueidentifier", 16, false, true);
        AddRow(t, "PromptId", "uniqueidentifier", 16, false);
        AddRow(t, "Name", "nvarchar", 200, false);
        AddRow(t, "Description", "nvarchar", 4000, true);
        AddRow(t, "IsActive", "bit", 1, false);
        AddRow(t, "CreatedAt", "datetime", 8, false);
        AddRow(t, "UpdatedAt", "datetime", 8, false);
        return t;
    }

    private static DataTable SchemaTestCase()
    {
        var t = CreateSchemaTable();
        AddRow(t, "Id", "uniqueidentifier", 16, false, true);
        AddRow(t, "SuiteId", "uniqueidentifier", 16, false);
        AddRow(t, "Name", "nvarchar", 200, false);
        AddRow(t, "InputVariables", "nvarchar", -1, false);
        AddRow(t, "ExpectedOutput", "nvarchar", -1, true);
        AddRow(t, "IsActive", "bit", 1, false);
        AddRow(t, "CreatedAt", "datetime", 8, false);
        AddRow(t, "UpdatedAt", "datetime", 8, false);
        return t;
    }

    private static DataTable SchemaTestRun()
    {
        var t = CreateSchemaTable();
        AddRow(t, "Id", "uniqueidentifier", 16, false, true);
        AddRow(t, "SuiteId", "uniqueidentifier", 16, false);
        AddRow(t, "PromptId", "uniqueidentifier", 16, false);
        AddRow(t, "PromptVersion", "int", 4, false);
        AddRow(t, "Model", "nvarchar", 200, false);
        AddRow(t, "Temperature", "decimal", 9, false);
        AddRow(t, "MaxTokens", "int", 4, true);
        AddRow(t, "Status", "nvarchar", 50, false);
        AddRow(t, "StartedAt", "datetime", 8, true);
        AddRow(t, "CompletedAt", "datetime", 8, true);
        AddRow(t, "CreatedAt", "datetime", 8, false);
        AddRow(t, "PromptTitle", "nvarchar", 500, true);
        AddRow(t, "SuiteName", "nvarchar", 500, true);
        return t;
    }

    private static DataTable SchemaTestResult()
    {
        var t = CreateSchemaTable();
        AddRow(t, "Id", "uniqueidentifier", 16, false, true);
        AddRow(t, "RunId", "uniqueidentifier", 16, false);
        AddRow(t, "CaseId", "uniqueidentifier", 16, false);
        AddRow(t, "ActualOutput", "nvarchar", -1, false);
        AddRow(t, "Passed", "bit", 1, false);
        AddRow(t, "Score", "decimal", 9, false);
        AddRow(t, "LatencyMs", "int", 4, false);
        AddRow(t, "Error", "nvarchar", 4000, true);
        AddRow(t, "CreatedAt", "datetime", 8, false);
        AddRow(t, "CaseName", "nvarchar", 500, true);
        AddRow(t, "InputVariables", "nvarchar", -1, true);
        AddRow(t, "ExpectedOutput", "nvarchar", -1, true);
        return t;
    }
}
