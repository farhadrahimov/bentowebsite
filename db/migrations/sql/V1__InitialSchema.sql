/*
  Tortcu - Initial Schema (V1)
  Execute on MSSQL database (e.g. TortcuDb)
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.Category', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Category
    (
        Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Category PRIMARY KEY,
        Name          NVARCHAR(200) NOT NULL,
        Slug          NVARCHAR(220) NOT NULL,
        IsActive      BIT NOT NULL CONSTRAINT DF_Category_IsActive DEFAULT (1),
        DisplayOrder  INT NOT NULL CONSTRAINT DF_Category_DisplayOrder DEFAULT (0)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Category_Slug' AND object_id = OBJECT_ID(N'dbo.Category'))
    CREATE UNIQUE INDEX UX_Category_Slug ON dbo.Category (Slug);
GO

IF OBJECT_ID(N'dbo.Product', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Product
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Product PRIMARY KEY,
        CategoryId   INT NOT NULL,
        Name         NVARCHAR(240) NOT NULL,
        Slug         NVARCHAR(260) NOT NULL,
        [Description] NVARCHAR(4000) NULL,
        Price        DECIMAL(18,2) NOT NULL CONSTRAINT DF_Product_Price DEFAULT (0),
        IsActive     BIT NOT NULL CONSTRAINT DF_Product_IsActive DEFAULT (1),
        IsPopular    BIT NOT NULL CONSTRAINT DF_Product_IsPopular DEFAULT (0),
        CreatedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_Product_CreatedAtUtc DEFAULT (sysutcdatetime())
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Product_Category')
BEGIN
    ALTER TABLE dbo.Product
      ADD CONSTRAINT FK_Product_Category
      FOREIGN KEY (CategoryId) REFERENCES dbo.Category(Id)
      ON DELETE NO ACTION;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Product_Slug' AND object_id = OBJECT_ID(N'dbo.Product'))
    CREATE UNIQUE INDEX UX_Product_Slug ON dbo.Product (Slug);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Product_CategoryId' AND object_id = OBJECT_ID(N'dbo.Product'))
    CREATE INDEX IX_Product_CategoryId ON dbo.Product (CategoryId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Product_IsPopular' AND object_id = OBJECT_ID(N'dbo.Product'))
    CREATE INDEX IX_Product_IsPopular ON dbo.Product (IsPopular) INCLUDE (IsActive);
GO

IF OBJECT_ID(N'dbo.ProductImage', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductImage
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductImage PRIMARY KEY,
        ProductId    INT NOT NULL,
        ImageUrl     NVARCHAR(600) NOT NULL,
        IsPrimary    BIT NOT NULL CONSTRAINT DF_ProductImage_IsPrimary DEFAULT (0),
        DisplayOrder INT NOT NULL CONSTRAINT DF_ProductImage_DisplayOrder DEFAULT (0)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_ProductImage_Product')
BEGIN
    ALTER TABLE dbo.ProductImage
      ADD CONSTRAINT FK_ProductImage_Product
      FOREIGN KEY (ProductId) REFERENCES dbo.Product(Id)
      ON DELETE CASCADE;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ProductImage_ProductId' AND object_id = OBJECT_ID(N'dbo.ProductImage'))
    CREATE INDEX IX_ProductImage_ProductId ON dbo.ProductImage (ProductId);
GO

IF OBJECT_ID(N'dbo.Campaign', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Campaign
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Campaign PRIMARY KEY,
        Title        NVARCHAR(200) NOT NULL,
        SubTitle     NVARCHAR(400) NULL,
        ImageUrl     NVARCHAR(600) NULL,
        StartDateUtc DATETIME2 NULL,
        EndDateUtc   DATETIME2 NULL,
        IsActive     BIT NOT NULL CONSTRAINT DF_Campaign_IsActive DEFAULT (0)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Campaign_IsActive' AND object_id = OBJECT_ID(N'dbo.Campaign'))
    CREATE INDEX IX_Campaign_IsActive ON dbo.Campaign (IsActive);
GO

IF OBJECT_ID(N'dbo.AboutContent', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AboutContent
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AboutContent PRIMARY KEY,
        Content      NVARCHAR(12000) NOT NULL,
        MainImageUrl NVARCHAR(600) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.SeoMeta', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SeoMeta
    (
        Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SeoMeta PRIMARY KEY,
        PageType        NVARCHAR(60) NOT NULL,
        EntityId        INT NULL,
        MetaTitle       NVARCHAR(300) NULL,
        MetaDescription NVARCHAR(600) NULL,
        MetaKeywords    NVARCHAR(600) NULL,
        OgTitle         NVARCHAR(300) NULL,
        OgDescription   NVARCHAR(600) NULL,
        OgImageUrl      NVARCHAR(600) NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_SeoMeta_PageType_EntityId' AND object_id = OBJECT_ID(N'dbo.SeoMeta'))
    CREATE UNIQUE INDEX UX_SeoMeta_PageType_EntityId ON dbo.SeoMeta (PageType, EntityId);
GO

