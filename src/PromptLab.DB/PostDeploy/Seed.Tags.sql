/*
  Tags adicionales para datos de volumen / performance (idempotente).
*/

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'hr')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'HR';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'legal')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Legal';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'finance')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Finance';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'product')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Product';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'data-analysis')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Data Analysis';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'content')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Content';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'operations')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Operations';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'devops')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'DevOps';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'security')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Security';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'testing')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Testing';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'onboarding')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Onboarding';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'reporting')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Reporting';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'customer-success')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Customer Success';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'ux-research')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'UX Research';
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = N'compliance')
BEGIN
    EXEC [dbo].[Tag_Create] @Name = N'Compliance';
END;
