CREATE TABLE [dbo].[PageWidget] (
    [PageWidgetUID] UNIQUEIDENTIFIER CONSTRAINT [DF_PageWidget_PageWidgetUID] DEFAULT (newid()) NOT NULL,
    [PageUID]       UNIQUEIDENTIFIER NOT NULL,
    [ModuleUID]     UNIQUEIDENTIFIER NOT NULL,
    [Title]         VARCHAR (128)    NULL,
    [Content]       VARCHAR (MAX)    NULL,
    [Col]           INT              CONSTRAINT [DF_PageWidget_Col] DEFAULT ((1)) NOT NULL,
    [Row]           INT              CONSTRAINT [DF_PageWidget_Row] DEFAULT ((1)) NOT NULL,
    [Width]         INT              CONSTRAINT [DF_PageWidget_Width] DEFAULT ((1)) NOT NULL,
    [Height]        INT              CONSTRAINT [DF_PageWidget_Height] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_PageWidget] PRIMARY KEY CLUSTERED ([PageWidgetUID] ASC),
    CONSTRAINT [FK_PageWidget_Module] FOREIGN KEY ([ModuleUID]) REFERENCES [dbo].[Module] ([ModuleUID]) ON DELETE CASCADE,
    CONSTRAINT [FK_PageWidget_Page] FOREIGN KEY ([PageUID]) REFERENCES [dbo].[Page] ([PageUID]) ON DELETE CASCADE
);



