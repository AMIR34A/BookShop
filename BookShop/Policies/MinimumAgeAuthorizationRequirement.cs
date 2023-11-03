using Microsoft.AspNetCore.Authorization;

namespace BookShop.Policies;

public class MinimumAgeAuthorizationRequirement : IAuthorizationRequirement
{
    public MinimumAgeAuthorizationRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }

    public int MinimumAge { get; set; }
}