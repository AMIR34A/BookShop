using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ProductsController : Controller
    {
        [Authorize(Policy = "NeedMinimumAge")]
        public IActionResult ProductsNeedRequirement() => View();
    }
}
