using BookShop.Areas.Admin.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Areas.Identity.Data;

public class ApplicationRoleManager : RoleManager<ApplicationRole>, IApplicationRoleManager
{
    private readonly IRoleStore<ApplicationRole> _roleStore;
    private readonly IEnumerable<IRoleValidator<ApplicationRole>> _roleValidators;
    private readonly ILookupNormalizer _lookupNormalizer;
    private readonly IdentityErrorDescriber _identityErrorDescriber;
    private readonly ILogger<ApplicationRoleManager> _logger;

    public ApplicationRoleManager(IRoleStore<ApplicationRole> store,
        IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
        ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
        ILogger<ApplicationRoleManager> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
    {
        _roleStore = store;
        _roleValidators = roleValidators;
        _lookupNormalizer = keyNormalizer;
        _identityErrorDescriber = errors;
        _logger = logger;
    }

    public List<ApplicationRole> GetAllRoles() => Roles.ToList();

    public List<RolesViewModel> GetAllRolesAndUsersCount()
    {
        return Roles.Select(role => new RolesViewModel
        {
            RoleId = role.Id,
            RoleName = role.Name,
            Description = role.Description,
            UserCount = role.Users.Count
        }).ToList();
    }

    public async Task<ApplicationRole> FindClaimsInRolesAsync(string roleId) => await Roles.Include(role => role.Claims).FirstOrDefaultAsync(role => role.Id == roleId);

    public async Task<IdentityResult> AddOrUpdateClaimsAsync(string roleId, string roleClaimType, string[] selectedRoleClaims)
    {
        var role = await FindClaimsInRolesAsync(roleId);
        if (role is null)
            return IdentityResult.Failed(new IdentityError()
            {
                Code = "NotFound",
                Description ="نقش مورد نظر یافت نشد"
            });


        var currentClaimValues = role.Claims.Where(roleClaim => roleClaim.ClaimType == roleClaimType).Select(roleClaim => roleClaim.ClaimValue);
        if (currentClaimValues.Count() == 0)
            currentClaimValues = new List<string>();

        var addeddClaimValues = selectedRoleClaims.Except(currentClaimValues);
        foreach (var claim in addeddClaimValues)
        {
            role.Claims.Add(new ApplicationRoleClaim
            {
                RoleId = roleId,
                ClaimType = roleClaimType,
                ClaimValue = claim
            });
        }

        var removedClaimValues = currentClaimValues.Except(selectedRoleClaims);
        foreach (var claim in removedClaimValues)
        {
            var removedClaim = role.Claims.SingleOrDefault(roleClaim => roleClaim.ClaimValue == claim && roleClaim.ClaimType == roleClaimType);
            role.Claims.Remove(removedClaim);
        }

        return await UpdateAsync(role);
    }
}