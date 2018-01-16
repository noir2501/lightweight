CREATE TABLE [dbo].[Role] (
    [RoleUID]      UNIQUEIDENTIFIER CONSTRAINT [DF_Role_RoleUID] DEFAULT (newid()) NOT NULL,
    [Name]         NVARCHAR (50)    NOT NULL,
    [TenantUID]    UNIQUEIDENTIFIER NULL,
    [Icon]         VARCHAR (128)    NULL,
    [IsSystemRole] BIT              CONSTRAINT [DF_Role_IsSystemRole] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleUID] ASC),
    CONSTRAINT [FK_Role_Tenant] FOREIGN KEY ([TenantUID]) REFERENCES [dbo].[Tenant] ([TenantUID]),
    CONSTRAINT [UK_Role_Name_Tenant] UNIQUE NONCLUSTERED ([Name] ASC, [TenantUID] ASC)
);



