using BookShop.Areas.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookShop.Areas.API.Services;

public class JwtService : IJwtService
{
    private readonly IApplicationUserManager _applicationUserManager;

    public JwtService(IApplicationUserManager applicationUserManager) => _applicationUserManager = applicationUserManager;

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

        var roles = await _applicationUserManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }
}
