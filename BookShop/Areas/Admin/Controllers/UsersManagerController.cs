using BookShop.Areas.Identity.Data;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
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
        else if (!string.IsNullOrEmpty(message) && message.Equals("SucceededEdit"))
            ViewBag.Message = "تغیرات با موفقیت ثبت شد.";

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UsersViewModel userViewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(userViewModel.Id);
            if (user is null)
                return NotFound();

            IdentityResult identityResult;
            var recentRoles = await _userManager.GetRolesAsync(user);
            var deletedRoles = recentRoles.Except(userViewModel.Roles);
            var addedRoles = userViewModel.Roles.Except(recentRoles);

            identityResult = await _userManager.RemoveFromRolesAsync(user, deletedRoles);
            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddToRolesAsync(user, addedRoles);
                if (identityResult.Succeeded)
                {
                    user.FirstName = userViewModel.FirstName;
                    user.LastName = userViewModel.LastName;
                    userViewModel.Username = userViewModel.Username;
                    user.Email = userViewModel.Email;
                    user.PhoneNumber = userViewModel.PhoneNumber;
                    user.BirthDate = userViewModel.BirthDate;
                }
                identityResult = await _userManager.UpdateAsync(user);
                if (identityResult.Succeeded)
                {
                    ViewBag.Message = "تغییرات با موفقیت اعمال شد.";
                    return RedirectToAction("Index", new { message = "SucceededEdit" });
                }
            }
            if (identityResult is null)
                foreach (var error in identityResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
        }
        ViewBag.Roles = _roleManager.GetAllRoles();
        return View(userViewModel);
    }
}
