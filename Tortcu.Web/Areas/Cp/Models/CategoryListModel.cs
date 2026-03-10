namespace Tortcu.Web.Areas.Cp.Models;

public sealed record CategoryListModel(int Id, string Name, string Slug, bool IsActive, int DisplayOrder, int ProductCount);
