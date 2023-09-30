using BookShop.Areas.Identity.Data;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace BookShop.Controllers;

public class AccountController : Controller
{
    private readonly IApplicationUserManager _userManager;
    private readonly IApplicationRoleManager _roleManager;
    private readonly IEmailSender _emailSender;
    public AccountController(IApplicationUserManager userManager, IApplicationRoleManager roleManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(AccountViewModel accountViewModel)
    {
        if (ModelState.IsValid)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = accountViewModel.Username,
                Email = accountViewModel.Email,
                PhoneNumber = accountViewModel.PhoneNumber,
                RegisterDate = DateTime.Now
            };
            IdentityResult identityResult = await _userManager.CreateAsync(user, accountViewModel.Password);

            if (identityResult.Succeeded)
            {
                var isExistRole = await _roleManager.RoleExistsAsync("کاربر");
                if (!isExistRole)
                    await _roleManager.CreateAsync(new ApplicationRole("کاربر"));

                identityResult = await _userManager.AddToRoleAsync(user, "کاربر");
                if (identityResult.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token },Request.Scheme);
                    string message = @$"<div dir='rtl' style='font-familt:tahoma;font-s-ize:14px'>
                                             لطفا برای تایید ایمیل کاربری روی لینک زیر کلیک کنید:
                                             <a href='{callbackUrl}'>تایید ایمیل</a>
                                         </div>";
                    await _emailSender.SendEmailAsync(accountViewModel.Email, "تایید ایمیل حساب کاربری - سایت کتاب", message);
                    return RedirectToAction("Index", "Home", new { message = "SendingEmailSucceeded" });
                }

            }
            foreach (var error in identityResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }
        return View();
    }

    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound($"Unable to find user with Id : '{userId}'");

        IdentityResult identityResult = await _userManager.ConfirmEmailAsync(user, token);
        if (!identityResult.Succeeded)
            throw new InvalidOperationException($"Error confirming users's email with Id : '{userId}'");
        return View();
    }
}
