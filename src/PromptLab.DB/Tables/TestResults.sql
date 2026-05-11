CREATE TABLE [dbo].[TestResults]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_TestResults_Id] DEFAULT NEWID(),
    [RunId] UNIQUEIDENTIFIER NOT NULL,
    [CaseId] UNIQUEIDENTIFIER NOT NULL,
    [ActualOutput] NVARCHAR(MAX) NOT NULL CONSTRAINT [DF_TestResults_ActualOutput] DEFAULT (N''),
    [Passed] BIT NOT NULL CONSTRAINT [DF_TestResults_Passed] DEFAULT (0),
    [Score] DECIMAL(9, 4) NOT NULL CONSTRAINT [DF_TestResults_Score] DEFAULT (0),
    [LatencyMs] INT NOT NULL CONSTRAINT [DF_TestResults_LatencyMs] DEFAULT (0),
    [Error] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2(3) NOT NULL CONSTRAINT [DF_TestResults_CreatedAt] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_TestResults] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TestResults_TestRuns] FOREIGN KEY ([RunId]) REFERENCES [dbo].[TestRuns] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TestResults_TestCases] FOREIGN KEY ([CaseId]) REFERENCES [dbo].[TestCases] ([Id])
);

GO

CREATE NONCLUSTERED INDEX [IX_TestResults_RunId]
    ON [dbo].[TestResults]([RunId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_TestResults_CaseId]
    ON [dbo].[TestResults]([CaseId] ASC);
