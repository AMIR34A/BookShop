using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProvincesController : Controller
    {
        BookShopContext _context;
        public ProvincesController(BookShopContext bookShopContext)
        {
            _context = bookShopContext;
        }

        public async Task<IActionResult> Index()
        {
            var provinces =  _context.Provinces.AsAsyncEnumerable();
            return View(provinces);
        }
    }
}
