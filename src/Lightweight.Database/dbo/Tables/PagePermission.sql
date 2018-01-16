CREATE TABLE [dbo].[PagePermission] (
    [PagePermissionUID] UNIQUEIDENTIFIER CONSTRAINT [DF_PagePermission_PagePermissionUID] DEFAULT (newid()) NOT NULL,
    [PageUID]           UNIQUEIDENTIFIER NOT NULL,
    [RoleUID]           UNIQUEIDENTIFIER NOT NULL,
    [View]              BIT              CONSTRAINT [DF_PagePermission_View] DEFAULT ((0)) NOT NULL,
    [Edit]              BIT              CONSTRAINT [DF_PagePermission_Edit] DEFAULT ((0)) NOT NULL,
    [Delete]            BIT              CONSTRAINT [DF_PagePermission_Delete] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PagePermission] PRIMARY KEY CLUSTERED ([PagePermissionUID] ASC),
    CONSTRAINT [FK_PagePermission_Page] FOREIGN KEY ([PageUID]) REFERENCES [dbo].[Page] ([PageUID]) ON DELETE CASCADE,
    CONSTRAINT [FK_PagePermission_Role] FOREIGN KEY ([RoleUID]) REFERENCES [dbo].[Role] ([RoleUID]) ON DELETE CASCADE,
    CONSTRAINT [UK_PagePermission_Page_Role] UNIQUE NONCLUSTERED ([PageUID] ASC, [RoleUID] ASC)
);

