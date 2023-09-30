using Microsoft.AspNetCore.Identity;

namespace BookShop.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BookShopUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Image { get; set; }
    public DateTime RegisterDate { get; set; }
    public DateTime LastViewDateTime { get; set; }
    public bool IsActive { get; set; }
    public virtual List<ApplicationUserRoles> Roles { get; set; }
}

