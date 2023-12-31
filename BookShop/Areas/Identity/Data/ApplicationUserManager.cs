﻿using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

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
        return await Users.Select(user => new UsersViewModel
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
            LockoutEnd = user.LockoutEnd,
            TwoFactorEnabled = user.TwoFactorEnabled,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            Roles = user.Roles.Select(u => u.Role.Name),

        }).ToListAsync();
    }

    public async Task<UsersViewModel> FindUserWithRolesByIdAsync(string id)
    {
        return await Users.Where(user => user.Id == id).Select(user => new UsersViewModel
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
            Roles = user.Roles.Select(u => u.Role.Name),
        }).FirstAsync();
    }

    public async Task<string> GetFullName(ClaimsPrincipal principal)
    {
        var userInfo = await GetUserAsync(principal);
        return $"{userInfo.FirstName} {userInfo.LastName}";
    }
    public string NormalizeKey(string key)
    {
        throw new NotImplementedException();
    }
}
