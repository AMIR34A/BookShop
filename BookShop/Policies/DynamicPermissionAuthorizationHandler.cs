using BookShop.Areas.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookShop.Policies;


public class DynamicPermissionAuthorizationHandler : AuthorizationHandler<DynamicPermissionAuthorizationRequirement>
{
    private readonly ISecurityTrimmingService _securityTrimmingService;
    public DynamicPermissionAuthorizationHandler(ISecurityTrimmingService securityTrimmingService) => _securityTrimmingService = securityTrimmingService;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DynamicPermissionAuthorizationRequirement requirement)
    {
        var filterContext = context.Resource as AuthorizationFilterContext;
        if (filterContext is null)
            context.Fail();

        var actionDescriptor = filterContext.ActionDescriptor;
        bool hasExistAreaName = actionDescriptor.RouteValues.TryGetValue("area", out string areaName);
        string area = hasExistAreaName ? areaName : string.Empty;

        bool hasExistControllerName = actionDescriptor.RouteValues.TryGetValue("controller", out string controllerName);
        string controller = hasExistControllerName ? controllerName : string.Empty;

        bool hasExistActionName = actionDescriptor.RouteValues.TryGetValue("action", out string actionName);
        string action = hasExistActionName ? actionName : string.Empty;

        bool hasUserAccess = _securityTrimmingService.CanCurrentUserAccess(area, controller, action);
        if (hasUserAccess)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
