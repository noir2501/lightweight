﻿CREATE TABLE [dbo].[UserRole] (
    [UserUID] UNIQUEIDENTIFIER NOT NULL,
    [RoleUID] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([UserUID] ASC, [RoleUID] ASC),
    CONSTRAINT [FK_UserRole_Role] FOREIGN KEY ([RoleUID]) REFERENCES [dbo].[Role] ([RoleUID]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRole_User] FOREIGN KEY ([UserUID]) REFERENCES [dbo].[User] ([UserUID]) ON DELETE CASCADE
);
