CREATE TABLE [dbo].[PromptTags]
(
    [PromptId] UNIQUEIDENTIFIER NOT NULL,
    [TagId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2(3) NOT NULL CONSTRAINT [DF_PromptTags_CreatedAt] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_PromptTags] PRIMARY KEY CLUSTERED ([PromptId] ASC, [TagId] ASC),
    CONSTRAINT [FK_PromptTags_Prompts_PromptId] FOREIGN KEY ([PromptId]) REFERENCES [dbo].[Prompts]([Id]),
    CONSTRAINT [FK_PromptTags_Tags_TagId] FOREIGN KEY ([TagId]) REFERENCES [dbo].[Tags]([Id])
);

GO

CREATE NONCLUSTERED INDEX [IX_PromptTags_TagId]
    ON [dbo].[PromptTags]([TagId] ASC);
