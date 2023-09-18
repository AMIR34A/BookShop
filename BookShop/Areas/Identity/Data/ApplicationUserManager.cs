using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BookShop.Areas.Identity.Data;

public class ApplicationUserManager : UserManager<ApplicationUser> ,IApplicationUserManager
{
    private readonly IUserStore<ApplicationUser> _store;
    private readonly IOptions<IdentityOptions> _optionsAccessor;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IEnumerable<IUserValidator<ApplicationUser>> _userValidators;
    private readonly IEnumerable<IPasswordValidator<ApplicationUser>> _passwordValidators;
    private readonly ILookupNormalizer _keyNormalizer;
    private readonly ApplicationIdentityErrorDescriber _errors;
    private readonly IServiceProvider _services;
    private readonly ILogger<ApplicationUserManager> _logger;
    public ApplicationUserManager(
        IUserStore<ApplicationUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        ApplicationIdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<ApplicationUserManager> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _store = store;
        _optionsAccessor = optionsAccessor;
        _passwordHasher = passwordHasher;
        _userValidators = userValidators;
        _keyNormalizer = keyNormalizer;
        _errors = errors;
        _services = services;
        _logger = logger;
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await Users.ToListAsync();
    }

    public string NormalizeKey(string key)
    {
        throw new NotImplementedException();
    }
}
