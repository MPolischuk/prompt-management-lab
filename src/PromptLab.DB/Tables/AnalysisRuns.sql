CREATE TABLE [dbo].[AnalysisRuns]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_AnalysisRuns_Id] DEFAULT NEWID(),
    [PromptId] UNIQUEIDENTIFIER NOT NULL,
    [Provider] NVARCHAR(50) NOT NULL,
    [ModelId] NVARCHAR(100) NULL,
    [Input] NVARCHAR(MAX) NULL,
    [Output] NVARCHAR(MAX) NULL,
    [Temperature] DECIMAL(4, 2) NULL,
    [MaxTokens] INT NULL,
    [TopP] DECIMAL(4, 2) NULL,
    [PromptSnapshot] NVARCHAR(MAX) NULL,
    [PromptSnapshotHash] NVARCHAR(128) NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [ErrorMessage] NVARCHAR(2000) NULL,
    [LatencyMs] INT NULL,
    [CreatedAt] DATETIME2(3) NOT NULL CONSTRAINT [DF_AnalysisRuns_CreatedAt] DEFAULT SYSUTCDATETIME(),
    [CompletedAt] DATETIME2(3) NULL,
    CONSTRAINT [PK_AnalysisRuns] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AnalysisRuns_Prompts_PromptId] FOREIGN KEY ([PromptId]) REFERENCES [dbo].[Prompts]([Id]),
    CONSTRAINT [CK_AnalysisRuns_Temperature_Range] CHECK ([Temperature] IS NULL OR ([Temperature] >= 0 AND [Temperature] <= 2)),
    CONSTRAINT [CK_AnalysisRuns_MaxTokens_Positive] CHECK ([MaxTokens] IS NULL OR [MaxTokens] > 0),
    CONSTRAINT [CK_AnalysisRuns_TopP_Range] CHECK ([TopP] IS NULL OR ([TopP] > 0 AND [TopP] <= 1))
);

GO

CREATE NONCLUSTERED INDEX [IX_AnalysisRuns_PromptId]
    ON [dbo].[AnalysisRuns]([PromptId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_AnalysisRuns_CreatedAt]
    ON [dbo].[AnalysisRuns]([CreatedAt] DESC);
