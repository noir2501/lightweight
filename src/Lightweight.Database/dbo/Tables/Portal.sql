CREATE TABLE [dbo].[Portal] (
    [TenantUID] UNIQUEIDENTIFIER NOT NULL,
    [Title]     NVARCHAR (256)   NOT NULL,
    [Url]       VARCHAR (256)    NOT NULL,
    CONSTRAINT [PK_Portal_1] PRIMARY KEY CLUSTERED ([TenantUID] ASC),
    CONSTRAINT [FK_Portal_Tenant] FOREIGN KEY ([TenantUID]) REFERENCES [dbo].[Tenant] ([TenantUID])
);

