CREATE TABLE [dbo].[UserProfile] (
    [UserUID]     UNIQUEIDENTIFIER NOT NULL,
    [FirstName]   NVARCHAR (50)    NULL,
    [LastName]    NVARCHAR (50)    NULL,
    [Email]       VARCHAR (50)     NULL,
    [Address]     NVARCHAR (256)   NULL,
    [State]       NVARCHAR (50)    NULL,
    [City]        NVARCHAR (50)    NULL,
    [Zip]         NVARCHAR (50)    NULL,
    [Country]     NVARCHAR (50)    NULL,
    [Phone]       VARCHAR (50)     NULL,
    [Mobile]      VARCHAR (50)     NULL,
    [Photo]       VARCHAR (256)    NULL,
    [Description] NVARCHAR (256)   NULL,
    [LastUpdated] DATETIME         CONSTRAINT [DF_UserProfile_LastUpdated] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED ([UserUID] ASC),
    CONSTRAINT [FK_UserProfile_User] FOREIGN KEY ([UserUID]) REFERENCES [dbo].[User] ([UserUID]) ON DELETE CASCADE
);

