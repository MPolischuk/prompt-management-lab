CREATE PROCEDURE [dbo].[TestCase_Create]
    @SuiteId UNIQUEIDENTIFIER,
    @Name NVARCHAR(200),
    @InputVariables NVARCHAR(MAX),
    @ExpectedOutput NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[TestSuites] WHERE [Id] = @SuiteId AND [IsActive] = 1)
    BEGIN
        SELECT CAST(0 AS BIT) AS [Success], CAST(NULL AS UNIQUEIDENTIFIER) AS [EntityId], N'Test suite not found.' AS [Message];
        RETURN;
    END

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @UtcNow DATETIME2(3) = SYSUTCDATETIME();

    INSERT INTO [dbo].[TestCases] ([Id], [SuiteId], [Name], [InputVariables], [ExpectedOutput], [IsActive], [CreatedAt], [UpdatedAt])
    VALUES (@Id, @SuiteId, @Name, @InputVariables, @ExpectedOutput, 1, @UtcNow, @UtcNow);

    SELECT CAST(1 AS BIT) AS [Success], @Id AS [EntityId], CAST(NULL AS NVARCHAR(500)) AS [Message];
END;
