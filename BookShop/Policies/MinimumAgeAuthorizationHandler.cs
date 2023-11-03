using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookShop.Policies;

public class MinimumAgeAuthorizationHandler : AuthorizationHandler<MinimumAgeAuthorizationRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeAuthorizationRequirement requirement)
    {
        if (!context.User.HasClaim(claim => claim.Type == ClaimTypes.DateOfBirth))
        {
            context.Fail();
            await Task.CompletedTask;
        }
        DateTime dateOfBirth = Convert.ToDateTime(context.User.FindFirstValue(ClaimTypes.DateOfBirth));
        int minimumDays = requirement.MinimumAge * 365;
        if (DateTime.Now.Subtract(dateOfBirth).TotalDays > minimumDays)
            context.Succeed(requirement);
    }
}
