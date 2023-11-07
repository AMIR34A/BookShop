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

    public async Task<ApplicationRole> FindClaimsInRoles(string roleId) => await Roles.FirstOrDefaultAsync(role => role.Id == roleId);
}