CREATE PROCEDURE [dbo].[TestResult_GetByRunId]
    @RunId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        tr.[Id],
        tr.[RunId],
        tr.[CaseId],
        tr.[ActualOutput],
        tr.[Passed],
        tr.[Score],
        tr.[LatencyMs],
        tr.[Error],
        tr.[CreatedAt],
        tc.[Name] AS [CaseName],
        tc.[InputVariables],
        tc.[ExpectedOutput]
    FROM [dbo].[TestResults] tr
    INNER JOIN [dbo].[TestCases] tc ON tc.[Id] = tr.[CaseId]
    WHERE tr.[RunId] = @RunId
    ORDER BY tr.[CreatedAt] ASC;
END;
