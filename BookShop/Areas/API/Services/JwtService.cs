using BookShop.Areas.Admin.Data;
using BookShop.Areas.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookShop.Areas.API.Services;

public class JwtService : IJwtService
{
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IApplicationRoleManager _applicationRoleManager;

    public JwtService(IApplicationUserManager applicationUserManager, IApplicationRoleManager applicationRoleManager)
    {
        _applicationUserManager = applicationUserManager;
        _applicationRoleManager = applicationRoleManager;
    }

    public async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        byte[] secretKey = Encoding.UTF8.GetBytes("1234567890abcdefghijklmnopqrstuvwxyz");
        var signinCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

        byte[] bytes = Encoding.UTF8.GetBytes("1234567890abcdef");
        var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(bytes), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

        var tokenDescriotir = new SecurityTokenDescriptor
        {
            Issuer = "BookShop.ir",
            Audience = "BookShop.ir",
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now,
            Expires = DateTime.Now.AddMinutes(20),
            SigningCredentials = signinCredentials,
            Subject = new ClaimsIdentity(await GetClaimsAsync(user)),
            EncryptingCredentials = encryptingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateJwtSecurityToken(tokenDescriotir);
        return tokenHandler.WriteToken(securityToken);
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

        var roles = _applicationRoleManager.Roles.ToList();

        foreach (var role in roles)
        {
            var roleClaims = await _applicationRoleManager.FindClaimsInRolesAsync(role.Id);
            foreach (var claim in roleClaims.Claims)
                claims.Add(new Claim(ConstantPolicies.DynamicPermission, claim.ClaimValue));
        }

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
        return claims;
    }
}
