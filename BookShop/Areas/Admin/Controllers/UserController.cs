using BookShop.Areas.Admin.Models.ViewModels;
using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace BookShop.Areas.Admin.Controllers;

[Area("Admin")]
public class UserController : Controller
{
    private readonly IApplicationUserManager _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UrlEncoder _urlEncoder;

    public UserController(IApplicationUserManager userManager, SignInManager<ApplicationUser> signInManager, UrlEncoder urlEncoder)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _urlEncoder = urlEncoder;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EnableAuthenticator()
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user is null)
            return NotFound();
        var unFormattedKey = _userManager.GenerateNewAuthenticatorKey();
        string formattedKey = FormatKey(unFormattedKey);
        EnableAuthenticatorViewModel enableAuthenticatorViewModel = new EnableAuthenticatorViewModel
        {
            AuthenticatorUri = GenerateQRCodeUri(unFormattedKey, user.Email),
            SharedKey = FormatKey(formattedKey)
        };
        return View(enableAuthenticatorViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel enableAuthenticatorViewModel)
    {
        if (!ModelState.IsValid)
            return View(enableAuthenticatorViewModel);

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return NotFound();

        var verificationCode = enableAuthenticatorViewModel.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
        bool isVerificationCodeValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
        if (!isVerificationCodeValid)
        {
            ModelState.AddModelError(string.Empty, "کد وارد شده نامعتبر است!!!");
            return View(enableAuthenticatorViewModel);
        }
        int recovryCodeCount = await _userManager.CountRecoveryCodesAsync(user);
        if (recovryCodeCount == 0)
        {
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 5);
            return View("ShowRecoveryCodes");
        }
        return RedirectToAction("TwoFactorAuthentication", new { message = "Success" });
    }

    public async Task<IActionResult> TwoFactorAuthentication(string message)
    {
        ViewBag.Messgae = message switch
        {
            "Success" => "اپیکیشن احراز هویت شا با موفقیت تایید شد.",
            _ => string.Empty
        };

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return NotFound();
        TwoFactorAuthenticationViewModel twoFactorAuthenticationViewModel = new TwoFactorAuthenticationViewModel
        {
            RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            HasAuthenticator = string.IsNullOrEmpty(await _userManager.GetAuthenticatorKeyAsync(user)),
            Is2FAEnabled = await _userManager.GetTwoFactorEnabledAsync(user)
        };
        return View(twoFactorAuthenticationViewModel);
    }

    public string FormatKey(string key)
    {
        var seperated = key.Chunk(4);
        return string.Join(" ", seperated);
    }

    public string GenerateQRCodeUri(string unformattedKey, string email)
    {
        string authenticatorFormat = "otpauth://totp/{0}/:{1}/?secret={2}/&issuer={0}&digits=6";
        return string.Format(authenticatorFormat, _urlEncoder.Encode("BookShop"), _urlEncoder.Encode(email), unformattedKey);
    }
}
