using BookShop.Areas.Admin.Models.ViewModels;
using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace BookShop.Areas.Admin.Controllers;

public class UserController : Controller
{
    private readonly IApplicationUserManager _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UrlEncoder _urlEncoder;

    public UserController(IApplicationUserManager userManager, SignInManager<ApplicationUser> signInManager, UrlEncoder urlEncoder)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        urlEncoder = urlEncoder;
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
