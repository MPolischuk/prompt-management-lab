CREATE PROCEDURE [dbo].[Prompt_Delete]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Prompts]
       SET [IsActive] = 0,
           [UpdatedAt] = SYSUTCDATETIME()
     WHERE [Id] = @Id;

    SELECT
        CASE WHEN @@ROWCOUNT > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS [Success],
        @Id AS [EntityId],
        CASE WHEN @@ROWCOUNT > 0 THEN CAST(NULL AS NVARCHAR(500)) ELSE N'Prompt not found.' END AS [Message];
END;
