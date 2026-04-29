CREATE TABLE [dbo].[AnalysisRuns]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_AnalysisRuns_Id] DEFAULT NEWID(),
    [PromptId] UNIQUEIDENTIFIER NOT NULL,
    [Provider] NVARCHAR(50) NOT NULL,
    [Input] NVARCHAR(MAX) NULL,
    [Output] NVARCHAR(MAX) NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [ErrorMessage] NVARCHAR(2000) NULL,
    [LatencyMs] INT NULL,
    [CreatedAt] DATETIME2(3) NOT NULL CONSTRAINT [DF_AnalysisRuns_CreatedAt] DEFAULT SYSUTCDATETIME(),
    [CompletedAt] DATETIME2(3) NULL,
    CONSTRAINT [PK_AnalysisRuns] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AnalysisRuns_Prompts_PromptId] FOREIGN KEY ([PromptId]) REFERENCES [dbo].[Prompts]([Id])
);

GO

CREATE NONCLUSTERED INDEX [IX_AnalysisRuns_PromptId]
    ON [dbo].[AnalysisRuns]([PromptId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_AnalysisRuns_CreatedAt]
    ON [dbo].[AnalysisRuns]([CreatedAt] DESC);
