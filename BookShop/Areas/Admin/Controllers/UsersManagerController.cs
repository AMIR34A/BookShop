using BookShop.Areas.Identity.Data;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace BookShop.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersManagerController : Controller
{
    private readonly IApplicationUserManager _userManager;
    private readonly IApplicationRoleManager _roleManager;
    private readonly IEmailSender _emailSender;

    public UsersManagerController(IApplicationUserManager userManager, IApplicationRoleManager roleManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
    }

    public async Task<IActionResult> Index(string? message, int pageIndex = 1, int pageSize = 10)
    {
        string mes = message switch
        {
            "success" => "کاربر با موفقیت اضافه شد.",
            "SucceededEdit" => "تغیرات با موفقیت ثبت شد.",
            "SucceededDeleted" => "کاربر با موفقیت حذف شد.",
            "SucceededEmailSender" => "عملیات ارسال ایمیل با موفقیت انجام شد.",
            "SucceededEditPassword" => "پسورد با موفقیت بازنشانی شد.",
            _ => ""
        };
        ViewBag.Message = mes;

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
                    return RedirectToAction("Index", new { message = "SucceededEdit" });
            }
            if (identityResult is null)
                foreach (var error in identityResult.Errors)
                    ModelState.AddModelError("", error.Description);
        }
        ViewBag.Roles = _roleManager.GetAllRoles();
        return View(userViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();
        return View(user);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deleted(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        IdentityResult identityResult = await _userManager.DeleteAsync(user);
        if (identityResult.Succeeded)
            return RedirectToAction("Index", new { message = "SucceededDeelted" });
        foreach (var error in identityResult.Errors)
            ModelState.AddModelError("", error.Description);
        return View();
    }

    public async Task<IActionResult> SendEmail(string[] emails, string subject, string message)
    {
        for (int i = 0; i < emails.Length; i++)
            await _emailSender.SendEmailAsync(emails[i], subject, message);
        return RedirectToAction("Index", new { message = "SucceededEmailSender" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeLockOutEnabled(string id, bool status)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();
        await _userManager.SetLockoutEnabledAsync(user, status);
        return RedirectToAction("Details", new { id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LockAndUnLockUserAccount(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();
        if (user.LockoutEnd.HasValue && DateTime.Compare(user.LockoutEnd.Value.ToLocalTime().DateTime, DateTime.Now) > 0)
            await _userManager.SetLockoutEndDateAsync(user, null);
        else
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(20));
        return RedirectToAction("Details", new { id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActiveAndDeactiveUserAccount(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);
        return RedirectToAction("Details", new { id = id });
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();
        var viewModel = new ResetUserPasswordViewModel
        {
            Id = id,
            Email = user.Email,
            Username = user.UserName
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetUserPasswordViewModel resetUserPasswordViewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(resetUserPasswordViewModel.Id);
            await _userManager.RemovePasswordAsync(user);
            IdentityResult identityResult = await _userManager.AddPasswordAsync(user, resetUserPasswordViewModel.NewPassword);
            if (identityResult.Succeeded)
                return RedirectToAction("Index", new { message = "SucceededEditPassword" });
            resetUserPasswordViewModel.Email = user.Email;
            resetUserPasswordViewModel.Username = user.UserName;
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeTwoFactorEnabled(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();
        user.TwoFactorEnabled = !user.TwoFactorEnabled;
        IdentityResult identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
            ModelState.AddModelError(string.Empty, "مشکلی رخ داد!!!");
        return RedirectToAction("Details", new { id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeEmailConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();
        user.EmailConfirmed = !user.EmailConfirmed;
        IdentityResult identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
            ModelState.AddModelError(string.Empty, "مشکلی رخ داد!!!");
        return RedirectToAction("Details", new { id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePhoneNumberConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound();
        user.PhoneNumberConfirmed = !user.PhoneNumberConfirmed;
        IdentityResult identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
            ModelState.AddModelError(string.Empty, "مشکلی رخ داد!!!");
        return RedirectToAction("Details", new { id = id });
    }
}