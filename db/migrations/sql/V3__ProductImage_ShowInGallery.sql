/*
  ProductImage: ShowInGallery — Qalereya səhifəsində göstərmək üçün flag
*/

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.ProductImage') AND name = N'ShowInGallery')
BEGIN
    ALTER TABLE dbo.ProductImage
    ADD ShowInGallery BIT NOT NULL CONSTRAINT DF_ProductImage_ShowInGallery DEFAULT (0);
END
GO
