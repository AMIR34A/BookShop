using BookShop.Areas.Identity.Data;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UsersAPIController : ControllerBase
{
    private readonly IApplicationUserManager _applicationUserManager;
    private readonly IApplicationRoleManager _applicationRoleManager;

    public UsersAPIController(IApplicationUserManager applicationUserManager, IApplicationRoleManager applicationRoleManager)
    {
        _applicationUserManager = applicationUserManager;
        _applicationRoleManager = applicationRoleManager;
    }

    [HttpGet]
    public async Task<List<UsersViewModel>> Get() => await _applicationUserManager.GetAllUsersWithRolesAsync();

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var user = await _applicationUserManager.FindByIdAsync(id);

        if (user is null)
            return NotFound();
        return new JsonResult(user);
    }

    [HttpPost]
    public async Task<JsonResult> Register(RegisterViewModel registerViewModel)
    {
        DateTime birthDate = Convert.ToDateTime(registerViewModel.BirthDate);
        ApplicationUser user = new ApplicationUser
        {
            UserName = registerViewModel.Username,
            Email = registerViewModel.Email,
            PhoneNumber = registerViewModel.PhoneNumber,
            BirthDate = birthDate,
            RegisterDate = DateTime.Now
        };
        IdentityResult identityResult = await _applicationUserManager.CreateAsync(user, registerViewModel.Password);

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
}
