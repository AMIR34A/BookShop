using BookShop.Areas.Identity.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookShop.Areas.API.Services;

public class JwtService : IJwtService
{
    private readonly IApplicationUserManager _applicationUserManager;
    public JwtService(IApplicationUserManager applicationUserManager) => _applicationUserManager = applicationUserManager;

    public async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        return string.Empty;
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name,user.UserName),
            new Claim(ClaimTypes.NameIdentifier,user.Id),
            new Claim(ClaimTypes.MobilePhone,user.PhoneNumber),
            new Claim("SecurityStampClaimType",user.SecurityStamp),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };

        var roles = await _applicationUserManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }
}
