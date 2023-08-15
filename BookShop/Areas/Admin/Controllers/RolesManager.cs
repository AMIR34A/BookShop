using BookShop.Areas.Admin.Models.ViewModels;
using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace BookShop.Areas.Admin.Controllers;

[Area("Admin")]
public class RolesManager : Controller
{
    RoleManager<ApplicationRole> roleManager;
    public RolesManager(RoleManager<ApplicationRole> roleManager) => this.roleManager = roleManager;


    public IActionResult Index(string message, int page = 1, int row = 10)
    {
        if (!string.IsNullOrEmpty(message))
            TempData["Message"] = message;
        var roles = roleManager.Roles.Select(role => new RolesViewModel { RoleId = role.Id, RoleName = role.Name, Description = role.Description });
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
        bool isExist = await roleManager.RoleExistsAsync(rolesViewModel.RoleName);
        if(isExist)
        {
            ViewBag.Error = "این نقش در سیستم وجود دارد";
            return View(rolesViewModel);
        }
        var result = await roleManager.CreateAsync(new ApplicationRole(rolesViewModel.RoleName, rolesViewModel.Description));
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
            RoleName = role.Name,
            Description = role.Description
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
        bool isExist = await roleManager.RoleExistsAsync(rolesViewModel.RoleName);
        if(isExist)
        {
            ViewBag.Message = "این نقش در سیستم وجود دارد";
            return View(rolesViewModel);
        }
        role.Name = rolesViewModel.RoleName;
        role.Description = rolesViewModel.Description;

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
