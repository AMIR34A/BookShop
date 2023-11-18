using BookShop.Areas.Admin.Models.ViewModels;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BookShop.Areas.Identity.Data;

public interface IApplicationRoleManager
{
    #region BaseMethods
    IQueryable<ApplicationRole> Roles { get; }
    ILookupNormalizer KeyNormalizer { get; set; }
    IdentityErrorDescriber ErrorDescriber { get; set; }
    IList<IRoleValidator<ApplicationRole>> RoleValidators { get; }
    bool SupportsQueryableRoles { get; }
    bool SupportsRoleClaims { get; }
    Task<IdentityResult> CreateAsync(ApplicationRole role);
    Task<IdentityResult> DeleteAsync(ApplicationRole role);
    Task<ApplicationRole> FindByIdAsync(string roleId);
    Task<ApplicationRole> FindByNameAsync(string roleName);
    string NormalizeKey(string key);
    Task<bool> RoleExistsAsync(string roleName);
    Task<IdentityResult> UpdateAsync(ApplicationRole role);
    Task UpdateNormalizedRoleNameAsync(ApplicationRole role);
    Task<string> GetRoleNameAsync(ApplicationRole role);
    Task<IdentityResult> SetRoleNameAsync(ApplicationRole role, string name);
    #endregion


    List<ApplicationRole> GetAllRoles();
    List<RolesViewModel> GetAllRolesAndUsersCount();
    Task<ApplicationRole> FindClaimsInRolesAsync(string roleId);
    Task<IdentityResult> AddOrUpdateClaimsAsync(string roleId, string roleClaimType, string[] selectedRoleClaims);
    Task<List<UsersViewModel>> GetUsersInRoleAsync(string roleId);
}
