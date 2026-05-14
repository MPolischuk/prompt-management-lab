/*
  Test suites para ~75% de prompts activos (deterministico por Id), idempotente por nombre.
*/

DECLARE @PromptId UNIQUEIDENTIFIER;
DECLARE @SuiteName NVARCHAR(200);
DECLARE @SuiteDescription NVARCHAR(1000) = N'Suite automatica de seed de volumen / performance.';

DECLARE suite_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT
    p.[Id],
    N'Suite seed v1: ' + LEFT(p.[Title], 180)
FROM [dbo].[Prompts] AS p
WHERE
    p.[IsActive] = 1
    AND ABS(CHECKSUM(CAST(p.[Id] AS VARBINARY(16)))) % 4 <> 0
    AND NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestSuites] AS ts
        WHERE ts.[PromptId] = p.[Id]
          AND ts.[Name] LIKE N'Suite seed v1:%'
    );

OPEN suite_cursor;

FETCH NEXT FROM suite_cursor
INTO @PromptId, @SuiteName;

WHILE @@FETCH_STATUS = 0
BEGIN
    EXEC [dbo].[TestSuite_Create]
        @PromptId = @PromptId,
        @Name = @SuiteName,
        @Description = @SuiteDescription;

    FETCH NEXT FROM suite_cursor
    INTO @PromptId, @SuiteName;
END;

CLOSE suite_cursor;
DEALLOCATE suite_cursor;
