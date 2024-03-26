using BookShop.Areas.Identity.Data;
using BookShop.Models.ViewModels;
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
}
