using Microsoft.AspNetCore.Mvc;
using Tortcu.Infrastructure.ViewModels;

namespace Tortcu.Web.Controllers;

internal static class ControllerMetaExtensions
{
    public static void ApplyMeta(this Controller controller, SeoMetaViewModel meta)
    {
        controller.ViewData["MetaTitle"] = meta.Title;
        controller.ViewData["MetaDescription"] = meta.Description;
        controller.ViewData["MetaKeywords"] = meta.Keywords;
        controller.ViewData["OgTitle"] = meta.OgTitle;
        controller.ViewData["OgDescription"] = meta.OgDescription;
        controller.ViewData["OgImageUrl"] = meta.OgImageUrl;
        controller.ViewData["CanonicalUrl"] = meta.CanonicalUrl;
    }
}

