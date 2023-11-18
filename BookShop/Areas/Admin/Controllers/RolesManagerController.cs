using BookShop.Areas.Admin.Models.ViewModels;
using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace BookShop.Areas.Admin.Controllers;

[Area("Admin")]
public class RolesManagerController : Controller
{
    private readonly IApplicationRoleManager _roleManager;

    public RolesManagerController(IApplicationRoleManager roleManager) => _roleManager = roleManager;

    public IActionResult Index(string message, int page = 1, int row = 10)
    {
        if (!string.IsNullOrEmpty(message))
            TempData["Message"] = message;
        var roles = _roleManager.GetAllRolesAndUsersCount();
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
        var result = await _roleManager.CreateAsync(new ApplicationRole(rolesViewModel.RoleName, rolesViewModel.Description));
        if (result.Succeeded)
            return RedirectToAction("Index");

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);
        return View(rolesViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> EditRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
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
        var role = await _roleManager.FindByIdAsync(rolesViewModel.RoleId);
        if (role is null)
            return NotFound();
        role.Name = rolesViewModel.RoleName;
        role.Description = rolesViewModel.Description;

        var result = await _roleManager.UpdateAsync(role);
        if (result.Succeeded)
            return RedirectToAction("Index", new { message = "عملیات با موفقیت انجام شد" });

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);
        return View(rolesViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
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
        var role = await _roleManager.FindByIdAsync(id);
        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
            return RedirectToAction("Index", new { message = "عملیات با موفقیت انجام شد" });

        var rolesViewModel = new RolesViewModel
        {
            RoleId = role.Id,
            RoleName = role.Name
        };
        return View(rolesViewModel);
    }
}