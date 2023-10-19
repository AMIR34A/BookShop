using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.Admin.Controllers;

public class UserController : Controller
{
    private readonly IApplicationUserManager _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public IActionResult Index()
    {
        return View();
    }
}
