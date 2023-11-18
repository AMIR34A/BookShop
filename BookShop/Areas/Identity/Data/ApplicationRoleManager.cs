using BookShop.Areas.Admin.Models.ViewModels;
using BookShop.Models.ViewModels;
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
    private readonly IApplicationUserManager _userManager;

    public ApplicationRoleManager(IRoleStore<ApplicationRole> store,
        IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
        ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
        ILogger<ApplicationRoleManager> logger,
        IApplicationUserManager userManager) : base(store, roleValidators, keyNormalizer, errors, logger)
    {
        _roleStore = store;
        _roleValidators = roleValidators;
        _lookupNormalizer = keyNormalizer;
        _identityErrorDescriber = errors;
        _logger = logger;
        _userManager = userManager;
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


        var currentClaimValues = role.Claims.Where(roleClaim => roleClaim.ClaimType == roleClaimType).Select(roleClaim => roleClaim.ClaimValue).ToList();
        if (currentClaimValues.Count() == 0)
            currentClaimValues = new List<string>();

        if(selectedRoleClaims is not null)
        {
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
        }


        var removedClaimValues = currentClaimValues.Except(selectedRoleClaims);
        foreach (var claim in removedClaimValues)
        {
            var removedClaim = role.Claims.SingleOrDefault(roleClaim => roleClaim.ClaimValue == claim && roleClaim.ClaimType == roleClaimType);
            role.Claims.Remove(removedClaim);
        }

        return await UpdateAsync(role);
    }

    public async Task<List<UsersViewModel>> GetUsersInRoleAsync(string roleId)
    {
        var usersId = from role in Roles
                    where role.Id == roleId
                    from user in role.Users
                    select user.UserId;

        var users = await _userManager.Users.Where(user => usersId.Contains(user.Id))
            .Select(user => new UsersViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                IsActive = user.IsActive,
                Image = user.Image,
                RegisterDate = user.RegisterDate,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                TwoFactorEnabled = user.TwoFactorEnabled,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Roles = user.Roles.Select(u => u.Role.Name)
            }).AsNoTracking().ToListAsync();

        return users;
    }
}