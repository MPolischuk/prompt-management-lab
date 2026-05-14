/*
  Relaciones PromptTags segun categoria (2-3 tags por prompt, idempotente).
*/

CREATE TABLE [#CategoryTagMap]
(
    [Category] NVARCHAR(100) COLLATE DATABASE_DEFAULT NOT NULL,
    [Slug] NVARCHAR(120) COLLATE DATABASE_DEFAULT NOT NULL
);

INSERT INTO [#CategoryTagMap] ([Category], [Slug])
VALUES
(N'Sales', N'sales'),
(N'Sales', N'marketing'),
(N'Sales', N'general'),
(N'Marketing', N'marketing'),
(N'Marketing', N'content'),
(N'Marketing', N'general'),
(N'Engineering', N'coding'),
(N'Engineering', N'devops'),
(N'Engineering', N'security'),
(N'Support', N'support'),
(N'Support', N'customer-success'),
(N'Support', N'general'),
(N'Operations', N'operations'),
(N'Operations', N'devops'),
(N'Operations', N'reporting'),
(N'HR', N'hr'),
(N'HR', N'onboarding'),
(N'HR', N'compliance'),
(N'Product', N'product'),
(N'Product', N'ux-research'),
(N'Product', N'general'),
(N'Data', N'data-analysis'),
(N'Data', N'reporting'),
(N'Data', N'coding');

INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
SELECT
    p.[Id],
    t.[Id],
    SYSUTCDATETIME()
FROM [dbo].[Prompts] AS p
INNER JOIN [#CategoryTagMap] AS m
    ON m.[Category] = p.[Category]
INNER JOIN [dbo].[Tags] AS t
    ON t.[Slug] = m.[Slug]
WHERE
    p.[IsActive] = 1
    AND NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[PromptTags] AS pt
        WHERE pt.[PromptId] = p.[Id]
          AND pt.[TagId] = t.[Id]
    );

DROP TABLE [#CategoryTagMap];
