CREATE TABLE [dbo].[Page] (
    [PageUID]       UNIQUEIDENTIFIER CONSTRAINT [DF_Page_PageUID] DEFAULT (newid()) NOT NULL,
    [TenantUID]     UNIQUEIDENTIFIER NOT NULL,
    [ParentPageUID] UNIQUEIDENTIFIER NULL,
    [Name]          VARCHAR (64)     NOT NULL,
    [Title]         VARCHAR (128)    NOT NULL,
    [Slug]          VARCHAR (128)    NOT NULL,
    [Url]           VARCHAR (256)    NULL,
    [IconUrl]       VARCHAR (256)    NULL,
    [Order]         SMALLINT         CONSTRAINT [DF_Page_Order] DEFAULT ((0)) NOT NULL,
    [Published]     BIT              CONSTRAINT [DF_Page_Published] DEFAULT ((0)) NOT NULL,
    [MenuOnly]      BIT              CONSTRAINT [DF_Page_MenuOnly] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Page] PRIMARY KEY CLUSTERED ([PageUID] ASC),
    CONSTRAINT [FK_Page_Page] FOREIGN KEY ([ParentPageUID]) REFERENCES [dbo].[Page] ([PageUID]),
    CONSTRAINT [UK_Page_Slug_Tenant] UNIQUE NONCLUSTERED ([TenantUID] ASC, [Slug] ASC)
);



