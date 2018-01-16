CREATE TABLE [dbo].[User] (
    [UserUID]   UNIQUEIDENTIFIER CONSTRAINT [DF_User_UserUID] DEFAULT (newid()) NOT NULL,
    [UserName]  VARCHAR (50)     NOT NULL,
    [Email]     VARCHAR (50)     NOT NULL,
    [TenantUID] UNIQUEIDENTIFIER NOT NULL,
    [Hash]      NVARCHAR (255)   NOT NULL,
    [Enabled]   BIT              CONSTRAINT [DF_User_Enabled] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserUID] ASC),
    CONSTRAINT [FK_User_Tenant] FOREIGN KEY ([TenantUID]) REFERENCES [dbo].[Tenant] ([TenantUID]),
    CONSTRAINT [UK_Email_Tenant] UNIQUE NONCLUSTERED ([TenantUID] ASC, [Email] ASC),
    CONSTRAINT [UK_UserName_Tenant] UNIQUE NONCLUSTERED ([TenantUID] ASC, [UserName] ASC)
);



