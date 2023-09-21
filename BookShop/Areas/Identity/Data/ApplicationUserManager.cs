using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BookShop.Areas.Identity.Data;

public class ApplicationUserManager : UserManager<ApplicationUser>, IApplicationUserManager
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

    public async Task<List<UsersViewModel>> GetAllUsersWithRolesAsync()
    {
        var users = Users;
        return await Users.Select(user => new UsersViewModel
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = user.BirthDate,
            IsActive = user.IsActive,
            Image = user.Image,
            RegisterDate = user.RegisterDate,
            Roles = user.Roles.Select(u => u.Role.Name),

        }).ToListAsync();
    }

    public string NormalizeKey(string key)
    {
        throw new NotImplementedException();
    }
}
