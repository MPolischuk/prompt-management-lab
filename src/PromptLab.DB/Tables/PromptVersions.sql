CREATE TABLE [dbo].[PromptVersions]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_PromptVersions_Id] DEFAULT NEWID(),
    [PromptId] UNIQUEIDENTIFIER NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [Version] INT NOT NULL,
    [CreatedAt] DATETIME2(3) NOT NULL CONSTRAINT [DF_PromptVersions_CreatedAt] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_PromptVersions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PromptVersions_Prompts] FOREIGN KEY ([PromptId]) REFERENCES [dbo].[Prompts] ([Id]) ON DELETE CASCADE
);

GO

CREATE NONCLUSTERED INDEX [IX_PromptVersions_PromptId]
    ON [dbo].[PromptVersions]([PromptId] ASC, [Version] DESC);

GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_PromptVersions_PromptId_Version]
    ON [dbo].[PromptVersions]([PromptId] ASC, [Version] ASC);
