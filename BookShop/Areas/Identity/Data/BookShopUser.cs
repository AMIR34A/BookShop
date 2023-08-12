using Microsoft.AspNetCore.Identity;

namespace BookShop.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BookShopUser class
public class BookShopUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
}

