using BookShop.Areas.Identity.Data;
using BookShop.Models.ViewModels;
using BookShop.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShop.Controllers;

public class AccountController : Controller
{
    private readonly IApplicationUserManager _userManager;
    private readonly IApplicationRoleManager _roleManager;
    private readonly IEmailSender _emailSender;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ISMSSenderService _senderService;
    public AccountController(IApplicationUserManager userManager,
        IApplicationRoleManager roleManager,
        IEmailSender emailSender,
        SignInManager<ApplicationUser> signInManager,
        ISMSSenderService senderService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
        _signInManager = signInManager;
        _senderService = senderService;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (ModelState.IsValid)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = registerViewModel.Username,
                Email = registerViewModel.Email,
                PhoneNumber = registerViewModel.PhoneNumber,
                RegisterDate = DateTime.Now
            };
            IdentityResult identityResult = await _userManager.CreateAsync(user, registerViewModel.Password);

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
                    await _emailSender.SendEmailAsync(registerViewModel.Email, "تایید ایمیل حساب کاربری - سایت کتاب", message);
                    return RedirectToAction("Index", "Home", new { message = "SendingEmailSucceeded" });
                }

            }
            foreach (var error in identityResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }
        return View();
    }

    [HttpGet]
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
        if (!Captcha.ValidateCaptchaCode(signInViewModel.CaptchaCode, HttpContext))
        {
            ModelState.AddModelError(string.Empty, "کد امینتی وارد شده اشتباه است");
            return View(new SignInViewModel { Username = signInViewModel.Username, Password = signInViewModel.Password });
        }
        if (ModelState.IsValid)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(signInViewModel.Username, signInViewModel.Password, signInViewModel.RememberMe, true);
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "به دلیل تلاشهای ناموفق بیش از حد، حساب کاربری شما به مدت 20 دقیقه قفل میباشد");
                return View();
            }
            if (signInResult.RequiresTwoFactor)
                return RedirectToAction("SendCode", new { rememberMe = signInViewModel.RememberMe });
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

    [HttpGet]
    public IActionResult ForgetPassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordViewModel.Email);
            if (user is null)
                ModelState.AddModelError(string.Empty, "ایمیل وارد شده در سامانه ثبت نشده است!!");
            else
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                    ModelState.AddModelError(string.Empty, "ایمیل وارد شده تایید نشده است، ابتدا ایمیل خود را تایید کنید");
                else
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    string callback = Url.Action("ResetPassword", "Account", new { email = forgetPasswordViewModel.Email, token = token }, Request.Scheme);
                    string message = $"<p> برای بازنشانی کلمه عبور<a href='{callback}'>اینجا کلیک کنید</a> </p>";
                    await _emailSender.SendEmailAsync(forgetPasswordViewModel.Email, "بازنشانی پسورد - فروشگاه کتاب", message);
                    return RedirectToAction("ForgetPasswordConfirmation");
                }
            }
        }
        return View(forgetPasswordViewModel);
    }

    [HttpGet]
    public IActionResult ForgetPasswordConfirmation() => View();

    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            return NotFound();
        var viewModel = new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        };
        return View(viewModel);
    }

    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
            if (user is null)
                ModelState.AddModelError(string.Empty, "ایمیل وارد شده اشتباه میباشد!!");
            else
            {
                IdentityResult identityResult = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token, resetPasswordViewModel.Password);
                if (identityResult.Succeeded)
                    return View("ResetPasswordConfirmation");
                foreach (var error in identityResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(resetPasswordViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> SendCode(bool rememberMe)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user is null)
            return NotFound();

        var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
        var factorOption = userFactors.Select(factor => new SelectListItem(factor.Equals("Email") ? "ایمیل" : "تلفن", factor)).ToList();

        var viewModel = new SendCodeViewModel
        {
            Providers = factorOption,
            RememberMe = rememberMe,
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendCode(SendCodeViewModel sendCodeViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user is null)
            return NotFound();
        var token = await _userManager.GenerateTwoFactorTokenAsync(user, sendCodeViewModel.SelectedProvider);
        if (string.IsNullOrEmpty(token))
            return View("Error");

        string message = $"<p style='direction:rtl;font-size:14px;font-family:tahoma'> کد اعتبارسنجی : {token}</p>";
        if (sendCodeViewModel.SelectedProvider.Equals("Email"))
            await _emailSender.SendEmailAsync(user.Email, "اعتبارسنجی اکانت", message);
        else if (sendCodeViewModel.SelectedProvider.Equals("Phone"))
        {
            var result = await _senderService.SendSMS(token, user.PhoneNumber);
            if(!result)
            {
                ModelState.AddModelError(string.Empty, "در ارسال پیامک خطایی رخ داد!!!");
                return View();
            }
        }
        return RedirectToAction("VerifyCode", new { provider = sendCodeViewModel.SelectedProvider, rememberMe = sendCodeViewModel.RememberMe });
    }
}
