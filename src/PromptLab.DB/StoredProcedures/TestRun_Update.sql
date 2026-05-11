CREATE PROCEDURE [dbo].[TestRun_Update]
    @Id UNIQUEIDENTIFIER,
    @Status NVARCHAR(20),
    @StartedAt DATETIME2(3) = NULL,
    @CompletedAt DATETIME2(3) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[TestRuns]
       SET [Status] = @Status,
           [StartedAt] = @StartedAt,
           [CompletedAt] = @CompletedAt
     WHERE [Id] = @Id;

    DECLARE @RowsAffected INT = @@ROWCOUNT;

    SELECT
        CASE WHEN @RowsAffected > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS [Success],
        @Id AS [EntityId],
        CASE WHEN @RowsAffected > 0 THEN CAST(NULL AS NVARCHAR(500)) ELSE N'Test run not found.' END AS [Message];
END;
