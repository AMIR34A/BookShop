using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string message)
        {
            string msg = message switch
            {
                "SendingEmailSucceeded" => "ارسال ایمیل با موفقیت انجام شد؛ برای تایید ایمیل خود بروی لینک ارسال شده کلیک کنید.",
                _ => string.Empty
            };
            ViewBag.Message = msg;
            return View();
        }
    }
}