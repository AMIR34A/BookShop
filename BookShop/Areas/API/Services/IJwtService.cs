using BookShop.Areas.Identity.Data;

namespace BookShop.Areas.API.Services;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
}
