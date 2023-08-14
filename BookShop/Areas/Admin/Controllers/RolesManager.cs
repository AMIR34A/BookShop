using BookShop.Areas.Admin.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace BookShop.Areas.Admin.Controllers;

[Area("Admin")]
public class RolesManager : Controller
{
    RoleManager<IdentityRole> roleManager;
    public RolesManager(RoleManager<IdentityRole> roleManager) => this.roleManager = roleManager;


    public IActionResult Index(string message, int page = 1, int row = 10)
    {

        if (!string.IsNullOrEmpty(message))
            TempData["Message"] = message;
        var roles = roleManager.Roles.Select(role => new RolesViewModel { RoleId = role.Id, RoleName = role.Name });
        var pagingModel = PagingList.Create(roles, row, page);
        pagingModel.RouteValue = new RouteValueDictionary()
        {
            {"row" , row }
        };
        return View(pagingModel);
    }

    [HttpGet]
    public IActionResult AddRole()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRole(RolesViewModel rolesViewModel)
    {
        var result = await roleManager.CreateAsync(new IdentityRole(rolesViewModel.RoleName));
        if (result.Succeeded)
            return RedirectToAction("Index");
        ViewBag.Error = "مشکلی در ثبت اطلاعات رخ داد";
        return View(rolesViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> EditRole(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role is null)
            return NotFound();
        var rolesViewModel = new RolesViewModel()
        {
            RoleId = role.Id,
            RoleName = role.Name
        };
        return View(rolesViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRole(RolesViewModel rolesViewModel)
    {
        var role = await roleManager.FindByIdAsync(rolesViewModel.RoleId);
        if (role is null)
            return NotFound();
        role.Name = rolesViewModel.RoleName;
        var result = await roleManager.UpdateAsync(role);
        if (result.Succeeded)
            return RedirectToAction("Index", new { message = "عملیات با موفقیت انجام شد" });

        ViewBag.Message = "در ذخیره اطلاعات مشکلی بوجود امده است";
        return View(rolesViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role is null)
            return NotFound();
        var rolesViewModel = new RolesViewModel
        {
            RoleId = role.Id,
            RoleName = role.Name
        };
        return View(rolesViewModel);
    }

    [HttpPost]
    [ActionName("DeleteRole")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRoleConfirmed(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        var result = await roleManager.DeleteAsync(role);
        if (result.Succeeded)
            return RedirectToAction("Index", new { message = "عملیات با موفقیت انجام شد" });

        ViewBag.Error = "در حذف اطلاعات مشکلی بوجود امد";
        var rolesViewModel = new RolesViewModel
        {
            RoleId = role.Id,
            RoleName = role.Name
        };
        return View(rolesViewModel);
    }
}
