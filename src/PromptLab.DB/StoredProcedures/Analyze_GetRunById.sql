CREATE PROCEDURE [dbo].[Analyze_GetRunById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ar.[Id],
        ar.[PromptId],
        p.[Title] AS [PromptTitle],
        ar.[Provider],
        ar.[Input],
        ar.[Output],
        ar.[Status],
        ar.[ErrorMessage],
        ar.[LatencyMs],
        ar.[CreatedAt],
        ar.[CompletedAt]
    FROM [dbo].[AnalysisRuns] ar
    INNER JOIN [dbo].[Prompts] p ON p.[Id] = ar.[PromptId]
    WHERE ar.[Id] = @Id;
END;
