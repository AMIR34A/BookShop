using BookShop.Areas.Identity.Data;
using BookShop.Models.ViewModels;
using BookShop.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers;

public class AccountController : Controller
{
    private readonly IApplicationUserManager _userManager;
    private readonly IApplicationRoleManager _roleManager;
    private readonly IEmailSender _emailSender;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public AccountController(IApplicationUserManager userManager, IApplicationRoleManager roleManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register() => View();

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
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
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

    [HttpGet]
    public IActionResult SignIn() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(SignInViewModel signInViewModel)
    {
        if (ModelState.IsValid)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(signInViewModel.Username, signInViewModel.Password, signInViewModel.RememberMe, false);
            if (signInResult.Succeeded)
                return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError(string.Empty, "پسورد یا نام کاربری وارد شده اشتباه میباشد");
        return View(new SignInViewModel { Username = signInViewModel.Username });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Route("get-captcha-image")]
    public IActionResult GetCaptchaImage()
    {
        int width = 100;
        int height = 36;
        var captchaCode = Captcha.GenerateCaptchaCode();
        var result = Captcha.GenerateCaptchImage(width, height, captchaCode);
        HttpContext.Session.SetString("CaptchaCode", captchaCode);
        Stream stream = new MemoryStream(result.CaptchaByteData);
        return new FileStreamResult(stream, "image/png");
    }
}
