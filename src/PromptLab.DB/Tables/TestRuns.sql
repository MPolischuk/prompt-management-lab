CREATE TABLE [dbo].[TestRuns]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_TestRuns_Id] DEFAULT NEWID(),
    [SuiteId] UNIQUEIDENTIFIER NOT NULL,
    [PromptId] UNIQUEIDENTIFIER NOT NULL,
    [PromptVersion] INT NOT NULL,
    [Model] NVARCHAR(200) COLLATE DATABASE_DEFAULT NOT NULL,
    [Temperature] DECIMAL(4, 2) NOT NULL CONSTRAINT [DF_TestRuns_Temperature] DEFAULT (0.7),
    [Status] NVARCHAR(20) COLLATE DATABASE_DEFAULT NOT NULL CONSTRAINT [DF_TestRuns_Status] DEFAULT (N'pending'),
    [StartedAt] DATETIME2(3) NULL,
    [CompletedAt] DATETIME2(3) NULL,
    [CreatedAt] DATETIME2(3) NOT NULL CONSTRAINT [DF_TestRuns_CreatedAt] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_TestRuns] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TestRuns_TestSuites] FOREIGN KEY ([SuiteId]) REFERENCES [dbo].[TestSuites] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TestRuns_Prompts] FOREIGN KEY ([PromptId]) REFERENCES [dbo].[Prompts] ([Id]),
    CONSTRAINT [CK_TestRuns_Status] CHECK ([Status] IN (N'pending', N'running', N'completed', N'failed'))
);

GO

CREATE NONCLUSTERED INDEX [IX_TestRuns_SuiteId]
    ON [dbo].[TestRuns]([SuiteId] ASC, [CreatedAt] DESC);

GO

CREATE NONCLUSTERED INDEX [IX_TestRuns_PromptId]
    ON [dbo].[TestRuns]([PromptId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_TestRuns_CreatedAt]
    ON [dbo].[TestRuns]([CreatedAt] DESC);
