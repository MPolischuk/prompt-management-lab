CREATE PROCEDURE [dbo].[TestResult_Create]
    @RunId UNIQUEIDENTIFIER,
    @CaseId UNIQUEIDENTIFIER,
    @ActualOutput NVARCHAR(MAX),
    @Passed BIT,
    @Score DECIMAL(9, 4),
    @LatencyMs INT,
    @Error NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[TestRuns] WHERE [Id] = @RunId)
    BEGIN
        SELECT CAST(0 AS BIT) AS [Success], CAST(NULL AS UNIQUEIDENTIFIER) AS [EntityId], N'Test run not found.' AS [Message];
        RETURN;
    END

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @UtcNow DATETIME2(3) = SYSUTCDATETIME();

    INSERT INTO [dbo].[TestResults]
    (
        [Id], [RunId], [CaseId], [ActualOutput], [Passed], [Score], [LatencyMs], [Error], [CreatedAt]
    )
    VALUES
    (
        @Id, @RunId, @CaseId, @ActualOutput, @Passed, @Score, @LatencyMs, @Error, @UtcNow
    );

    SELECT CAST(1 AS BIT) AS [Success], @Id AS [EntityId], CAST(NULL AS NVARCHAR(500)) AS [Message];
END;
