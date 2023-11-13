using BookShop.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BookShop.Areas.Admin.TagHelpers;

public class SecurityTrimmingTagHelper : TagHelper
{
    private readonly ISecurityTrimmingService _securityTrimmingService;

    public SecurityTrimmingTagHelper(ISecurityTrimmingService securityTrimmingService)
    {
        _securityTrimmingService = securityTrimmingService;
    }

    [HtmlAttributeName("asp-area")]
    public string Area { get; set; }

    [HtmlAttributeName("asp-controller")]
    public string Controller { get; set; }

    [HtmlAttributeName("asp-action")]
    public string Action { get; set; }

    [ViewContext,HtmlAttributeNotBound]
    public ViewContext  ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = string.Empty;
        if (!ViewContext.HttpContext.User.Identity.IsAuthenticated)
            output.SuppressOutput();
        if (_securityTrimmingService.CanCurrentUserAccess(Area, Controller, Action))
            return;

        base.Process(context, output);
    }
}
