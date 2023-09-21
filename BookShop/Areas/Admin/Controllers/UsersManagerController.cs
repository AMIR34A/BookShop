using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace BookShop.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersManagerController : Controller
{
    private readonly IApplicationUserManager _userManager;

    public UsersManagerController(IApplicationUserManager userManager) => _userManager = userManager;

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
}
