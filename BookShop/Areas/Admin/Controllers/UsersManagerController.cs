using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace BookShop.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersManagerController : Controller
{
    private readonly IApplicationUserManager _userManager;
    private readonly IApplicationRoleManager _roleManager;

    public UsersManagerController(IApplicationUserManager userManager, IApplicationRoleManager roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index(string? message, int pageIndex = 1, int pageSize = 10)
    {
        if (!string.IsNullOrEmpty(message) && message.Equals("success"))
            ViewBag.Message = "کاربر با موفقیت اضافه شد.";

        var pagingModel = PagingList.Create(await _userManager.GetAllUsersWithRolesAsync(), pageSize, pageIndex);
        pagingModel.RouteValue = new RouteValueDictionary
        {
            ["paginSize"] = pageSize
        };
        return View(pagingModel);
    }

    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindUserWithRolesByIdAsync(id);
        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();
        var user = await _userManager.FindUserWithRolesByIdAsync(id);
        if (user is null)
            return NotFound();
        ViewBag.Roles = _roleManager.GetAllRoles();
        return View(user);
    }
}
