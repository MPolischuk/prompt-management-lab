CREATE PROCEDURE [dbo].[TestCase_Update]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(200),
    @InputVariables NVARCHAR(MAX),
    @ExpectedOutput NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[TestCases]
       SET [Name] = @Name,
           [InputVariables] = @InputVariables,
           [ExpectedOutput] = @ExpectedOutput,
           [UpdatedAt] = SYSUTCDATETIME()
     WHERE [Id] = @Id
       AND [IsActive] = 1;

    DECLARE @RowsAffected INT = @@ROWCOUNT;

    SELECT
        CASE WHEN @RowsAffected > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS [Success],
        @Id AS [EntityId],
        CASE WHEN @RowsAffected > 0 THEN CAST(NULL AS NVARCHAR(500)) ELSE N'Test case not found.' END AS [Message];
END;
