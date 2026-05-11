CREATE PROCEDURE [dbo].[TestCase_GetBySuiteId]
    @SuiteId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.[Id],
        c.[SuiteId],
        c.[Name],
        c.[InputVariables],
        c.[ExpectedOutput],
        c.[IsActive],
        c.[CreatedAt],
        c.[UpdatedAt]
    FROM [dbo].[TestCases] c
    WHERE c.[SuiteId] = @SuiteId
      AND c.[IsActive] = 1
    ORDER BY c.[CreatedAt] ASC;
END;
