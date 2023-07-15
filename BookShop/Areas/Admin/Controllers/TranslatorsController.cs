using BookShop.Models;
using BookShop.Models.ViewModels;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Areas.Admin.Controllers
{
    //[Area("Admin")]
    public class TranslatorsController : Controller
    {
        private readonly BookShopContext _context;

        public TranslatorsController(BookShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Translators.ToListAsync());
        }

        //[HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TranslatorsCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            Translator translator = new Translator
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName
            };
            _context.Add(translator);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var translator = await _context.Translators.FirstOrDefaultAsync(t => t.TranslatorId == id);
            if (translator == null)
                return NotFound();

            return View(translator);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Translator translator)
        {
            //if (!ModelState.IsValid)
            //    return NotFound();
            _context.Update(translator);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var translator = await _context.Translators.FirstAsync(t => t.TranslatorId == id.Value);
            return View(translator);
        }

        [HttpPost]
        public async Task<IActionResult> Deleted(int? id)
        {
            var translator = await _context.Translators.FindAsync(id.Value);
            _context.Translators.Remove(translator);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var traslator = await _context.Translators.FirstOrDefaultAsync(t => t.TranslatorId == id);
            return View(traslator);
        }
    }
}
