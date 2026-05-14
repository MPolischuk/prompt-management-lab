/*
  Cinco casos de prueba por suite seed (idempotente por SuiteId + nombre).
*/

DECLARE @SuiteId UNIQUEIDENTIFIER;

DECLARE case_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT [Id]
FROM [dbo].[TestSuites]
WHERE [Name] LIKE N'Suite seed v1:%'
  AND [IsActive] = 1;

OPEN case_cursor;

FETCH NEXT FROM case_cursor
INTO @SuiteId;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestCases]
        WHERE [SuiteId] = @SuiteId
          AND [Name] = N'Seed TC 01 - baseline'
    )
    BEGIN
        EXEC [dbo].[TestCase_Create]
            @SuiteId = @SuiteId,
            @Name = N'Seed TC 01 - baseline',
            @InputVariables = N'{"contexto":"Acme Corp","objetivo":"salida estable","restricciones":"max 200 tokens"}',
            @ExpectedOutput = N'[SEED] Salida esperada baseline sin caracteres especiales.';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestCases]
        WHERE [SuiteId] = @SuiteId
          AND [Name] = N'Seed TC 02 - vacios parciales'
    )
    BEGIN
        EXEC [dbo].[TestCase_Create]
            @SuiteId = @SuiteId,
            @Name = N'Seed TC 02 - vacios parciales',
            @InputVariables = N'{"contexto":"","objetivo":"probar defaults","restricciones":"ninguna"}',
            @ExpectedOutput = N'[SEED] Debe manejar strings vacios sin error.';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestCases]
        WHERE [SuiteId] = @SuiteId
          AND [Name] = N'Seed TC 03 - unicode y tildes'
    )
    BEGIN
        EXEC [dbo].[TestCase_Create]
            @SuiteId = @SuiteId,
            @Name = N'Seed TC 03 - unicode y tildes',
            @InputVariables = N'{"contexto":"Cliente con ñ y áéíóú","objetivo":"UTF-8","restricciones":"emoji permitidos"}',
            @ExpectedOutput = N'[SEED] Respuesta con caracteres Unicode correctos.';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestCases]
        WHERE [SuiteId] = @SuiteId
          AND [Name] = N'Seed TC 04 - valores largos'
    )
    BEGIN
        EXEC [dbo].[TestCase_Create]
            @SuiteId = @SuiteId,
            @Name = N'Seed TC 04 - valores largos',
            @InputVariables = N'{"contexto":"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris.","objetivo":"stress","restricciones":"sin truncar"}',
            @ExpectedOutput = N'[SEED] Debe procesar entradas largas sin degradar calidad.';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestCases]
        WHERE [SuiteId] = @SuiteId
          AND [Name] = N'Seed TC 05 - numeros y booleans'
    )
    BEGIN
        EXEC [dbo].[TestCase_Create]
            @SuiteId = @SuiteId,
            @Name = N'Seed TC 05 - numeros y booleans',
            @InputVariables = N'{"max_tokens":512,"temperature":0.73,"stream":false,"retry_count":3}',
            @ExpectedOutput = N'[SEED] JSON con tipos mixtos.';
    END;

    FETCH NEXT FROM case_cursor
    INTO @SuiteId;
END;

CLOSE case_cursor;
DEALLOCATE case_cursor;
