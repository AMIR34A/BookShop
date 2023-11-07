using Microsoft.AspNetCore.Identity;

namespace BookShop.Areas.Identity.Data;

public class ApplicationRoleClaim : IdentityRoleClaim<string>
{
    public ApplicationRole Role { get; set; }
}
