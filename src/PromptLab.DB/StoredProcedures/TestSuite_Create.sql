CREATE PROCEDURE [dbo].[TestSuite_Create]
    @PromptId UNIQUEIDENTIFIER,
    @Name NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Id] = @PromptId)
    BEGIN
        SELECT CAST(0 AS BIT) AS [Success], CAST(NULL AS UNIQUEIDENTIFIER) AS [EntityId], N'Prompt not found.' AS [Message];
        RETURN;
    END

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @UtcNow DATETIME2(3) = SYSUTCDATETIME();

    INSERT INTO [dbo].[TestSuites] ([Id], [PromptId], [Name], [Description], [IsActive], [CreatedAt], [UpdatedAt])
    VALUES (@Id, @PromptId, @Name, @Description, 1, @UtcNow, @UtcNow);

    SELECT CAST(1 AS BIT) AS [Success], @Id AS [EntityId], CAST(NULL AS NVARCHAR(500)) AS [Message];
END;
