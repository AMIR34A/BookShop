using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    public class ProductsController : Controller
    {
        [Authorize(Policy = "NeedMinimumAge")]
        public IActionResult ProductsNeedRequirement() => View();
    }
}
