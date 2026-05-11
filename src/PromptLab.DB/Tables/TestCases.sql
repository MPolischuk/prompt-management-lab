CREATE TABLE [dbo].[TestCases]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_TestCases_Id] DEFAULT NEWID(),
    [SuiteId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [InputVariables] NVARCHAR(MAX) NOT NULL CONSTRAINT [DF_TestCases_InputVariables] DEFAULT (N'{}'),
    [ExpectedOutput] NVARCHAR(MAX) NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_TestCases_IsActive] DEFAULT (1),
    [CreatedAt] DATETIME2(3) NOT NULL CONSTRAINT [DF_TestCases_CreatedAt] DEFAULT SYSUTCDATETIME(),
    [UpdatedAt] DATETIME2(3) NOT NULL CONSTRAINT [DF_TestCases_UpdatedAt] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_TestCases] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TestCases_TestSuites] FOREIGN KEY ([SuiteId]) REFERENCES [dbo].[TestSuites] ([Id]) ON DELETE CASCADE
);

GO

CREATE NONCLUSTERED INDEX [IX_TestCases_SuiteId]
    ON [dbo].[TestCases]([SuiteId] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_TestCases_IsActive]
    ON [dbo].[TestCases]([IsActive] ASC);
