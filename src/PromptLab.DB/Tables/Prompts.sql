CREATE TABLE [dbo].[Prompts]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Prompts_Id] DEFAULT NEWID(),
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [Category] NVARCHAR(100) NULL,
    [Language] NVARCHAR(20) NULL,
    [ModelHint] NVARCHAR(100) NULL,
    [TargetModelId] NVARCHAR(100) NULL,
    [Temperature] DECIMAL(4, 2) NULL,
    [MaxTokens] INT NULL,
    [TopP] DECIMAL(4, 2) NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_Prompts_IsActive] DEFAULT (1),
    [CreatedAt] DATETIME2(3) NOT NULL CONSTRAINT [DF_Prompts_CreatedAt] DEFAULT SYSUTCDATETIME(),
    [UpdatedAt] DATETIME2(3) NOT NULL CONSTRAINT [DF_Prompts_UpdatedAt] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_Prompts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_Prompts_Temperature_Range] CHECK ([Temperature] IS NULL OR ([Temperature] >= 0 AND [Temperature] <= 2)),
    CONSTRAINT [CK_Prompts_MaxTokens_Positive] CHECK ([MaxTokens] IS NULL OR [MaxTokens] > 0),
    CONSTRAINT [CK_Prompts_TopP_Range] CHECK ([TopP] IS NULL OR ([TopP] > 0 AND [TopP] <= 1))
);

GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_Prompts_Title]
    ON [dbo].[Prompts]([Title] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_Prompts_Category]
    ON [dbo].[Prompts]([Category] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_Prompts_Language]
    ON [dbo].[Prompts]([Language] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_Prompts_IsActive]
    ON [dbo].[Prompts]([IsActive] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_Prompts_CreatedAt]
    ON [dbo].[Prompts]([CreatedAt] DESC);
