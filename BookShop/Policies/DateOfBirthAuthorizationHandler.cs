using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookShop.Policies
{
    public class DateOfBirthAuthorizationHandler : AuthorizationHandler<DateOfBirthAuthorizationRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DateOfBirthAuthorizationRequirement requirement)
        {
            if (!context.User.HasClaim(claim => claim.Type == ClaimTypes.DateOfBirth))
                await Task.CompletedTask;

            DateTime dateOfBirth = Convert.ToDateTime(context.User.FindFirstValue(ClaimTypes.DateOfBirth));

            if (DateTime.Compare(new DateTime(DateTime.Now.Year, dateOfBirth.Month, dateOfBirth.Day), DateTime.Today) == 0)
                context.Succeed(requirement);
        }
    }
}
