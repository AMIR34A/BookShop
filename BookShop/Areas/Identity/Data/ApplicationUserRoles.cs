using Microsoft.AspNetCore.Identity;

namespace BookShop.Areas.Identity.Data
{
    public class ApplicationUserRoles : IdentityUserRole<string>
    {
        public virtual ApplicationRole Role { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
