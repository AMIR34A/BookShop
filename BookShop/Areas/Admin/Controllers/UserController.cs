using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.Admin.Controllers;

public class UserController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
