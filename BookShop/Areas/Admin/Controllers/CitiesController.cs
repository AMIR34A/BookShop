using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CitiesController : Controller
    {
        BookShopContext _context;

        public CitiesController(BookShopContext bookShopContext)
        {
            _context = bookShopContext;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var province =  _context.Provinces.Single(p => p.ProvinceId == id.Value);
            await _context.Entry(province).Collection(p=>p.Cities).LoadAsync();
            return View(province);
        }
    }
}
