using Microsoft.AspNetCore.Identity;

namespace BookShop.Areas.Identity.Data;

public class ApplicationIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return base.PasswordRequiresNonAlphanumeric();
    }
}
