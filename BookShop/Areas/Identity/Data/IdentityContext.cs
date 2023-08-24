using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Data;

public class IdentityContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRoles, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
{
    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationRole>().ToTable("AspNetRoles").ToTable("AppRoles");

        builder.Entity<ApplicationUserRoles>().ToTable("AppUserRoles");
        builder.Entity<ApplicationUserRoles>()
            .HasOne(userRole => userRole.Role)
            .WithMany(role => role.Users).HasForeignKey(userRole => userRole.RoleId);

        builder.Entity<ApplicationUser>().ToTable("AppUsers");
        builder.Entity<ApplicationUserRoles>()
            .HasOne(userRole => userRole.User)
            .WithMany(user => user.Roles).HasForeignKey(userRole => userRole.UserId);
    }
}
