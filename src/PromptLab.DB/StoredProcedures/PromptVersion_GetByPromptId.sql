CREATE PROCEDURE [dbo].[PromptVersion_GetByPromptId]
    @PromptId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.[Id],
        v.[PromptId],
        v.[Content],
        v.[Version],
        v.[CreatedAt]
    FROM [dbo].[PromptVersions] v
    WHERE v.[PromptId] = @PromptId
    ORDER BY v.[Version] DESC;
END;
