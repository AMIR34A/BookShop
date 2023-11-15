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
        var defaultContext = context.Resource as DefaultHttpContext;
        if (defaultContext is null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        //var actionDescriptor = filterContext.;
        string areaName = defaultContext.GetRouteValue("area") as string;
        string area = string.IsNullOrEmpty(areaName) ? string.Empty : areaName;

        string controllerName = defaultContext.GetRouteValue("controller") as string;
        string controller = string.IsNullOrEmpty(controllerName) ? string.Empty : controllerName;

        string actionName = defaultContext.GetRouteValue("action") as string;
        string action = string.IsNullOrEmpty(actionName) ? string.Empty : actionName;

        bool hasUserAccess = _securityTrimmingService.CanCurrentUserAccess(area, controller, action);
        if (hasUserAccess)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
