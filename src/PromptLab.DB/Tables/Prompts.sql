CREATE TABLE [dbo].[Prompts]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Prompts_Id] DEFAULT NEWID(),
    [Title] NVARCHAR(200) COLLATE DATABASE_DEFAULT NOT NULL,
    [Description] NVARCHAR(1000) COLLATE DATABASE_DEFAULT NULL,
    [Content] NVARCHAR(MAX) COLLATE DATABASE_DEFAULT NOT NULL,
    [Category] NVARCHAR(100) COLLATE DATABASE_DEFAULT NULL,
    [Language] NVARCHAR(20) COLLATE DATABASE_DEFAULT NULL,
    [ModelHint] NVARCHAR(100) COLLATE DATABASE_DEFAULT NULL,
    [TargetModelId] NVARCHAR(100) COLLATE DATABASE_DEFAULT NULL,
    [Temperature] DECIMAL(4, 2) NULL,
    [MaxTokens] INT NULL,
    [TopP] DECIMAL(4, 2) NULL,
    [Version] INT NOT NULL CONSTRAINT [DF_Prompts_Version] DEFAULT (1),
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
