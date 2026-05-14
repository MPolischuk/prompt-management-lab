CREATE PROCEDURE [dbo].[TestRun_Create]
    @SuiteId UNIQUEIDENTIFIER,
    @PromptId UNIQUEIDENTIFIER,
    @PromptVersion INT,
    @Model NVARCHAR(200),
    @Temperature DECIMAL(4, 2),
    @MaxTokens INT = NULL,
    @Status NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[TestSuites] WHERE [Id] = @SuiteId AND [IsActive] = 1)
    BEGIN
        SELECT CAST(0 AS BIT) AS [Success], CAST(NULL AS UNIQUEIDENTIFIER) AS [EntityId], N'Test suite not found.' AS [Message];
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Id] = @PromptId)
    BEGIN
        SELECT CAST(0 AS BIT) AS [Success], CAST(NULL AS UNIQUEIDENTIFIER) AS [EntityId], N'Prompt not found.' AS [Message];
        RETURN;
    END

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @UtcNow DATETIME2(3) = SYSUTCDATETIME();

    INSERT INTO [dbo].[TestRuns]
    (
        [Id], [SuiteId], [PromptId], [PromptVersion], [Model], [Temperature], [MaxTokens], [Status], [StartedAt], [CompletedAt], [CreatedAt]
    )
    VALUES
    (
        @Id, @SuiteId, @PromptId, @PromptVersion, @Model, @Temperature, @MaxTokens, @Status, NULL, NULL, @UtcNow
    );

    SELECT CAST(1 AS BIT) AS [Success], @Id AS [EntityId], CAST(NULL AS NVARCHAR(500)) AS [Message];
END;
