/*
  Seed basico opcional para desarrollo local.
*/

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'general')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'General';
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'marketing')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Marketing';
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'coding')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Coding';
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'sales')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Sales';
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'support')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Support';
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Title] = N'Email de seguimiento comercial')
BEGIN
    EXEC [dbo].[Prompt_Create]
        @Title = N'Email de seguimiento comercial',
        @Description = N'Genera un email de seguimiento para prospectos con tono profesional.',
        @Content = N'Escribe un email de seguimiento para {{nombre_cliente}} sobre {{producto}} destacando beneficios y proximo paso.',
        @Category = N'Sales',
        @Language = N'es',
        @ModelHint = N'gpt-4o-mini',
        @TargetModelId = N'gpt-5.5',
        @Temperature = 0.70,
        @MaxTokens = 350,
        @TopP = 1.00;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Title] = N'Resumen tecnico de incidente')
BEGIN
    EXEC [dbo].[Prompt_Create]
        @Title = N'Resumen tecnico de incidente',
        @Description = N'Construye un resumen tecnico estructurado para incidentes operativos.',
        @Content = N'Dado el incidente {{incidente_id}}, resume causa raiz, impacto, mitigacion y acciones de prevencion.',
        @Category = N'Operations',
        @Language = N'es',
        @ModelHint = N'claude-3-5-sonnet',
        @TargetModelId = N'claude-sonnet',
        @Temperature = 0.40,
        @MaxTokens = 500,
        @TopP = 0.95;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Title] = N'Asistente para documentacion de API')
BEGIN
    EXEC [dbo].[Prompt_Create]
        @Title = N'Asistente para documentacion de API',
        @Description = N'Genera documentacion breve de endpoints para equipos internos.',
        @Content = N'Para el endpoint {{metodo}} {{ruta}}, describe objetivo, parametros, respuestas y ejemplos de error.',
        @Category = N'Engineering',
        @Language = N'en',
        @ModelHint = N'gemini-1.5-pro',
        @TargetModelId = N'gemini-2.5-pro',
        @Temperature = 0.30,
        @MaxTokens = 600,
        @TopP = 0.90;
END

DECLARE @PromptSales UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Prompts]
    WHERE [Title] = N'Email de seguimiento comercial'
);

DECLARE @PromptIncident UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Prompts]
    WHERE [Title] = N'Resumen tecnico de incidente'
);

DECLARE @PromptApiDoc UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Prompts]
    WHERE [Title] = N'Asistente para documentacion de API'
);

DECLARE @TagGeneral UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Tags]
    WHERE [Slug] = N'general'
);

DECLARE @TagMarketing UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Tags]
    WHERE [Slug] = N'marketing'
);

DECLARE @TagCoding UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Tags]
    WHERE [Slug] = N'coding'
);

DECLARE @TagSales UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Tags]
    WHERE [Slug] = N'sales'
);

DECLARE @TagSupport UNIQUEIDENTIFIER =
(
    SELECT TOP 1 [Id]
    FROM [dbo].[Tags]
    WHERE [Slug] = N'support'
);

IF @PromptSales IS NOT NULL AND @TagSales IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [dbo].[PromptTags]
       WHERE [PromptId] = @PromptSales
         AND [TagId] = @TagSales
   )
BEGIN
    INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
    VALUES (@PromptSales, @TagSales, SYSUTCDATETIME());
END

IF @PromptSales IS NOT NULL AND @TagMarketing IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [dbo].[PromptTags]
       WHERE [PromptId] = @PromptSales
         AND [TagId] = @TagMarketing
   )
BEGIN
    INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
    VALUES (@PromptSales, @TagMarketing, SYSUTCDATETIME());
END

IF @PromptIncident IS NOT NULL AND @TagSupport IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [dbo].[PromptTags]
       WHERE [PromptId] = @PromptIncident
         AND [TagId] = @TagSupport
   )
BEGIN
    INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
    VALUES (@PromptIncident, @TagSupport, SYSUTCDATETIME());
END

IF @PromptApiDoc IS NOT NULL AND @TagCoding IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [dbo].[PromptTags]
       WHERE [PromptId] = @PromptApiDoc
         AND [TagId] = @TagCoding
   )
BEGIN
    INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
    VALUES (@PromptApiDoc, @TagCoding, SYSUTCDATETIME());
END

IF @PromptApiDoc IS NOT NULL AND @TagGeneral IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [dbo].[PromptTags]
       WHERE [PromptId] = @PromptApiDoc
         AND [TagId] = @TagGeneral
   )
BEGIN
    INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
    VALUES (@PromptApiDoc, @TagGeneral, SYSUTCDATETIME());
END

IF @PromptSales IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM [dbo].[AnalysisRuns]
       WHERE [PromptId] = @PromptSales
         AND [Provider] = N'simulated'
   )
BEGIN
    EXEC [dbo].[Analyze_CreateRun]
        @PromptId = @PromptSales,
        @Provider = N'simulated',
        @ModelId = N'simulated-default',
        @Input = N'nombre_cliente=Acme Corp; producto=PromptLab Enterprise',
        @Output = N'[SIMULATED] Email generado para seguimiento comercial con CTA.',
        @Temperature = 0.70,
        @MaxTokens = 350,
        @TopP = 1.00,
        @PromptSnapshot = N'Escribe un email de seguimiento para {{nombre_cliente}} sobre {{producto}} destacando beneficios y proximo paso.',
        @PromptSnapshotHash = N'SEED-HASH',
        @Status = N'Completed',
        @ErrorMessage = NULL,
        @LatencyMs = 142;
END
