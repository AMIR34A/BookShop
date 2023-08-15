using Microsoft.AspNetCore.Identity;

namespace BookShop.Areas.Identity.Data;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole() { }

    public ApplicationRole(string name) : base(name) { }

    public ApplicationRole(string name, string description) : base(name) => Description = description;

    public string Description { get; set; }
}
