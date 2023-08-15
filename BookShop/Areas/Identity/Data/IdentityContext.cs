using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Data;

public class IdentityContext : IdentityDbContext<BookShopUser, ApplicationRole, string>
{
    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationRole>().ToTable("AspNetRoles").ToTable("AppRoles");
    }
}
