CREATE PROCEDURE [dbo].[TestSuite_GetById]
    @Id UNIQUEIDENTIFIER
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
    WHERE s.[Id] = @Id
      AND s.[IsActive] = 1;
END;
