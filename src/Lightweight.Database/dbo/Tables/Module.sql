CREATE TABLE [dbo].[Module] (
    [ModuleUID]     UNIQUEIDENTIFIER CONSTRAINT [DF_Module_ModuleUID] DEFAULT (newid()) NOT NULL,
    [Name]          VARCHAR (128)    NOT NULL,
    [Configuration] VARCHAR (256)    NULL,
    CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED ([ModuleUID] ASC)
);



