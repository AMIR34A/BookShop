using BookShop.Areas.Admin.Data;
using System.Security.Claims;

namespace BookShop.Areas.Admin.Services;

public class SecurityTrimmingService : ISecurityTrimmingService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMVCActionsDiscoveryService _actionsDiscoveryService;
    public SecurityTrimmingService(IHttpContextAccessor httpContextAccessor, IMVCActionsDiscoveryService actionsDiscoveryService)
    {
        _httpContextAccessor = httpContextAccessor;
        _actionsDiscoveryService = actionsDiscoveryService;
    }

    public bool CanCurrentUserAccess(string area, string controller, string action) => CanUserAccess(_httpContextAccessor.HttpContext.User, area, controller, action);

    public bool CanUserAccess(ClaimsPrincipal user, string area, string controller, string action)
    {
        string currentClaimValue = $"{area}:{controller}:{action}";
        var securedControllerActions = _actionsDiscoveryService.GetAllSecuredControllerActionsWithPolicy(ConstantPolicies.DynamicPermission);
        bool isSecured = securedControllerActions.SelectMany(controller => controller.Actions)
            .Any(action => action.ActionId == currentClaimValue);
        if (!isSecured)
            throw new KeyNotFoundException("");

        if (!user.Identity.IsAuthenticated)
            return false;

        return user.HasClaim(claim => claim.Type == ConstantPolicies.DynamicPermissionClaimType && claim.Value == currentClaimValue);
    }
}
