using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersManagerController : Controller
{
    public IActionResult Index(string message)
    {
        if (message.Equals("success"))
            ViewBag.Message = "کاربر با موفقیت اضافه شد.";
        return View();
    }
}
