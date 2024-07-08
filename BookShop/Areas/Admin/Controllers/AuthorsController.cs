using BookShop.Models.Repository;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AuthorsController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public AuthorsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: Admin/Authors
        public async Task<IActionResult> Index()
        {
            var authors = await unitOfWork.RepositoryBase<Author>().GetAllAsync();
            return View(authors);
        }

        // GET: Admin/Authors/Details/5
        public async Task<IActionResult> Details(int? id)

        {
            if (id is null)
            {
                return NotFound();
            }

            var author = await unitOfWork.RepositoryBase<Author>().FindByIdAsync(id);
            if (author is null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Admin/Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorId,FirstName,LastName")] Author author)
        {
            if (!ModelState.IsValid)
            {
                await unitOfWork.RepositoryBase<Author>().CreateAsync(author);
                await unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Admin/Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var author = await unitOfWork.RepositoryBase<Author>().FindByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Admin/Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorId,FirstName,LastName")] Author author)
        {
            if (id != author.AuthorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.RepositoryBase<Author>().Update(author);
                    await unitOfWork.SaveAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await AuthorExists(author.AuthorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Admin/Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var author = await unitOfWork.RepositoryBase<Author>().FindByIdAsync(id);

            if (author is null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Admin/Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await unitOfWork.RepositoryBase<Author>().FindByIdAsync(id);
            if (author != null)
            {
                unitOfWork.RepositoryBase<Author>().Delete(author);
            }

            await unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> AuthorExists(int id)
        {
            return await unitOfWork.RepositoryBase<Author>().FindByIdAsync(id) is not null ? true : false;
        }

        public async Task<IActionResult> AuthorBooks(int id)
        {
            var authors = await unitOfWork.RepositoryBase<Author>().FindByIdAsync(id);

            if (authors is null)
                return NotFound();

            return View(authors);
        }
    }
}
