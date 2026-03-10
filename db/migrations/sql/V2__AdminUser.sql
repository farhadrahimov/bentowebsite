/*
  Tortcu - AdminUser schema (V2)
  Execute after V1__InitialSchema.sql
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.AdminUser', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AdminUser
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AdminUser PRIMARY KEY,
        Username     NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(256) NOT NULL,
        CreatedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_AdminUser_CreatedAtUtc DEFAULT (sysutcdatetime())
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_AdminUser_Username' AND object_id = OBJECT_ID(N'dbo.AdminUser'))
    CREATE UNIQUE INDEX UX_AdminUser_Username ON dbo.AdminUser (Username);
GO
