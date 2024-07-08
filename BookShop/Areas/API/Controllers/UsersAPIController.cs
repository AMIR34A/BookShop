using Asp.Versioning;
using BookShop.Areas.API.Classes;
using BookShop.Areas.API.Services;
using BookShop.Areas.Identity.Data;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.API.Controllers;

[Route("api/version:apiVersion/[controller]/[action]")]
[ApiController]
[APIResultFilter]
[ApiVersion("1.0")]
public class UsersAPIController : ControllerBase
{
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IApplicationRoleManager _applicationRoleManager;
    private readonly IJwtService _jwtService;

    public UsersAPIController(IApplicationUserManager applicationUserManager, IApplicationRoleManager applicationRoleManager, IJwtService jwtService)
    {
        _applicationUserManager = applicationUserManager;
        _applicationRoleManager = applicationRoleManager;
        _jwtService = jwtService;
    }

    [HttpGet]
    [Authorize]
    public async Task<APIResult<List<UsersViewModel>>> Get() => await _applicationUserManager.GetAllUsersWithRolesAsync();

    [HttpGet("{id}")]
    public async Task<APIResult<JsonResult>> Get(string id)
    {
        var user = await _applicationUserManager.FindByIdAsync(id);

        if (user is null)
            return BadRequest();
        return new JsonResult(user);
    }

    [HttpPost]
    public async Task<APIResult<JsonResult>> Register(RegisterBaseViewModel registerBaseViewModel)
    {
        ApplicationUser user = new ApplicationUser
        {
            UserName = registerBaseViewModel.Username,
            Email = registerBaseViewModel.Email,
            RegisterDate = DateTime.Now
        };
        IdentityResult identityResult = await _applicationUserManager.CreateAsync(user, registerBaseViewModel.Password);

        if (identityResult.Succeeded)
        {
            var isExistRole = await _applicationRoleManager.RoleExistsAsync("کاربر");
            if (!isExistRole)
                await _applicationRoleManager.CreateAsync(new ApplicationRole("کاربر"));

            identityResult = await _applicationUserManager.AddToRoleAsync(user, "کاربر");
            if (identityResult.Succeeded)
                return new JsonResult("عملیات با موفقیت انجام شد.");
        }
        return new JsonResult(identityResult.Errors);
    }

    [HttpPost]
    public async Task<APIResult<string>> SignIn(SignInBaseViewModel signInBaseViewModel)
    {
        var user = await _applicationUserManager.FindByNameAsync(signInBaseViewModel.Username);
        if (user is null)
            return BadRequest("کاربری با این نام کاربری یافت نشد!!!");
        var result = await _applicationUserManager.CheckPasswordAsync(user, signInBaseViewModel.Password);
        return result ? Ok(await _jwtService.GenerateTokenAsync(user)) : BadRequest("نام کاربری و یا پسورد اشتباه میباشد.");
    }
}
