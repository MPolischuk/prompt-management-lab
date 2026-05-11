CREATE PROCEDURE [dbo].[TestSuite_GetByPromptId]
    @PromptId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.[Id],
        s.[PromptId],
        s.[Name],
        s.[Description],
        s.[IsActive],
        s.[CreatedAt],
        s.[UpdatedAt]
    FROM [dbo].[TestSuites] s
    WHERE s.[PromptId] = @PromptId
      AND s.[IsActive] = 1
    ORDER BY s.[CreatedAt] DESC;
END;
