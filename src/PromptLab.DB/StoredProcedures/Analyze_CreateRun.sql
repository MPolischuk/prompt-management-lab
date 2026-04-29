CREATE PROCEDURE [dbo].[Analyze_CreateRun]
    @PromptId UNIQUEIDENTIFIER,
    @Provider NVARCHAR(50),
    @ModelId NVARCHAR(100) = NULL,
    @Input NVARCHAR(MAX) = NULL,
    @Output NVARCHAR(MAX) = NULL,
    @Temperature DECIMAL(4, 2) = NULL,
    @MaxTokens INT = NULL,
    @TopP DECIMAL(4, 2) = NULL,
    @PromptSnapshot NVARCHAR(MAX) = NULL,
    @PromptSnapshotHash NVARCHAR(128) = NULL,
    @Status NVARCHAR(20),
    @ErrorMessage NVARCHAR(2000) = NULL,
    @LatencyMs INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Id] = @PromptId)
    BEGIN
        SELECT CAST(0 AS BIT) AS [Success], CAST(NULL AS UNIQUEIDENTIFIER) AS [EntityId], N'Prompt not found.' AS [Message];
        RETURN;
    END

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @CompletedAt DATETIME2(3) = CASE WHEN @Status IN (N'Completed', N'Failed') THEN SYSUTCDATETIME() ELSE NULL END;

    INSERT INTO [dbo].[AnalysisRuns]
    (
        [Id], [PromptId], [Provider], [ModelId], [Input], [Output], [Temperature], [MaxTokens], [TopP], [PromptSnapshot], [PromptSnapshotHash], [Status], [ErrorMessage], [LatencyMs], [CreatedAt], [CompletedAt]
    )
    VALUES
    (
        @Id, @PromptId, @Provider, @ModelId, @Input, @Output, @Temperature, @MaxTokens, @TopP, @PromptSnapshot, @PromptSnapshotHash, @Status, @ErrorMessage, @LatencyMs, SYSUTCDATETIME(), @CompletedAt
    );

    SELECT CAST(1 AS BIT) AS [Success], @Id AS [EntityId], CAST(NULL AS NVARCHAR(500)) AS [Message];
END;
