using BookShop.Areas.Admin.Data;
using BookShop.Models;
using BookShop.Models.Repository;
using BookShop.Models.ViewModels;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System.ComponentModel;
using Publisher = EntityFrameworkCore.Models.Publisher;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin"), DisplayName("مدیریت کتاب‌ها")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BooksController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [DisplayName("مشاهده کتاب‌ها")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Index(string Message, int pageIndex, int row = 5, string sortExpression = "Title", string title = "")
        {
            if (!string.IsNullOrEmpty(Message))
                ViewBag.Message = "خطایی رخ داده است، لطفا مجدد تلاش کنید";
            //var books = (from b in _context.Books.AsEnumerable()
            //             join p in _context.Publishers on b.PublisherId equals p.PublisherId
            //             join u in _context.Author_Books on b.BookId equals u.BookId
            //             join a in _context.Authors on u.AuthorId equals a.AuthorId
            //             where b.IsDeleted == false
            //             select new BooksIndexViewModel
            //             {
            //                 BookId = b.BookId,
            //                 Title = b.Title,
            //                 ISBN = b.ISBN,
            //                 Price = b.Price,
            //                 Stock = b.Stock,
            //                 IsPublish = b.IsPublished,
            //                 PublishDate = b.PublishedTime,
            //                 PublisherName = p.PublisherName,
            //                 Authors = $"{u.Author.FirstName} {u.Author.LastName}"
            //             }).GroupBy(b => b.BookId).Select(g => new { BookId = g.Key, BookGroups = g }).ToList();

            List<int> rows = new List<int> { 1, 2, 5, 10, 20, 50, 100 };


            ViewBag.RowId = new SelectList(rows, row);
            ViewBag.NumOfRow = (pageIndex - 1) * row + 1;
            ViewBag.Search = title;

            title = string.IsNullOrEmpty(title) ? "" : title;

            var paging = PagingList.Create(unitOfWork.BooksRepository.GetAllBooks(title, "", "", "", "", "", ""), row, pageIndex, sortExpression, "Title");

            paging.RouteValue = new RouteValueDictionary()
            {
                {"row",row },
                {"title",title }
            };

            var languages = await unitOfWork.RepositoryBase<Language>().GetAllAsync();
            var publishers = await unitOfWork.RepositoryBase<Publisher>().GetAllAsync();
            var authors = await unitOfWork.RepositoryBase<Author>().GetAllAsync();
            var translators = await unitOfWork.RepositoryBase<Translator>().GetAllAsync();

            ViewBag.Language = new SelectList(languages, "LanguageName", "LanguageName");
            ViewBag.Publisher = new SelectList(publishers, "PublisherName", "PublisherName");
            ViewBag.Author = new SelectList(authors.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "NameFamily", "NameFamily");
            ViewBag.Translator = new SelectList(translators.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "NameFamily", "NameFamily");
            ViewBag.Categories = unitOfWork.BooksRepository.GetAllCategories();
            return View(paging);
        }

        public IActionResult AdvancedSearch(BooksAdvancedSearch booksAdvancedSearch)
        {
            booksAdvancedSearch.Title = booksAdvancedSearch.Title == null ? "" : booksAdvancedSearch.Title;
            booksAdvancedSearch.ISBN = booksAdvancedSearch.ISBN == null ? "" : booksAdvancedSearch.ISBN;
            booksAdvancedSearch.Language = booksAdvancedSearch.Language == null ? "" : booksAdvancedSearch.Language;
            booksAdvancedSearch.Author = booksAdvancedSearch.Author == null ? "" : booksAdvancedSearch.Author;
            booksAdvancedSearch.Translator = booksAdvancedSearch.Translator == null ? "" : booksAdvancedSearch.Translator;
            booksAdvancedSearch.Category = booksAdvancedSearch.Category == null ? "" : booksAdvancedSearch.Category;
            booksAdvancedSearch.Publisher = booksAdvancedSearch.Publisher == null ? "" : booksAdvancedSearch.Publisher;

            return View(unitOfWork.BooksRepository.GetAllBooks(booksAdvancedSearch.Title, booksAdvancedSearch.ISBN, booksAdvancedSearch.Language, booksAdvancedSearch.Author, booksAdvancedSearch.Translator, booksAdvancedSearch.Category, booksAdvancedSearch.Publisher));
        }

        [DisplayName("مشاهده جزئیات کتاب‌")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Details(int id)
        {
            //var bookInfo = _context.ReadAllBooks.Where(b => b.BookId == id).First();
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var book = await unitOfWork.RepositoryBase<Book>().FindByIdAsync(id);
            book.IsDeleted = true;
            await unitOfWork.SaveAsync();
            return RedirectToAction("Index");
        }

        [DisplayName("افزودن کتاب")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Create()
        {
            var languages = await unitOfWork.RepositoryBase<Language>().GetAllAsync();
            var publishers = await unitOfWork.RepositoryBase<EntityFrameworkCore.Models.Publisher>().GetAllAsync();
            var authors = await unitOfWork.RepositoryBase<Author>().GetAllAsync();
            var translators = await unitOfWork.RepositoryBase<Translator>().GetAllAsync();

            ViewBag.LanguageID = new SelectList(languages, "LanguageId", "LanguageName");
            ViewBag.PublisherId = new SelectList(publishers, "PublisherId", "PublisherName");
            ViewBag.AuthorID = new SelectList(authors.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(translators.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "TranslatorID", "NameFamily");
            BooksCreateEditViewModel viewModel = new BooksCreateEditViewModel() { BookSubCategoriesViewModel = new BookSubCategoriesViewModel(unitOfWork.BooksRepository.GetAllCategories(), null) };
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BooksCreateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var fileExtension = Path.GetExtension(viewModel.File.FileName);
                var newFileName = string.Concat(Guid.NewGuid(), fileExtension);
                var path = $"{_webHostEnvironment.WebRootPath}/BookFiles/{newFileName}";
                using FileStream stream = new FileStream(path, FileMode.Create);
                await viewModel.File.CopyToAsync(stream);
                viewModel.FileName = newFileName;
                if (await unitOfWork.BooksRepository.CreateBookAsync(viewModel))
                    return RedirectToAction("Index");
            }
            var languages = await unitOfWork.RepositoryBase<Language>().GetAllAsync();
            var publishers = await unitOfWork.RepositoryBase<Publisher>().GetAllAsync();
            var authors = await unitOfWork.RepositoryBase<Author>().GetAllAsync();
            var translators = await unitOfWork.RepositoryBase<Translator>().GetAllAsync();

            ViewBag.LanguageID = new SelectList(languages, "LanguageId", "LanguageName");
            ViewBag.PublisherId = new SelectList(publishers, "PublisherId", "PublisherName");
            ViewBag.AuthorID = new SelectList(authors.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(translators.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "TranslatorID", "NameFamily");
            viewModel.BookSubCategoriesViewModel = new BookSubCategoriesViewModel(unitOfWork.BooksRepository.GetAllCategories(), viewModel.CategoryID);
            return View(viewModel);
        }

        [HttpGet]
        [DisplayName("ویرایش کتاب‌")]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();
            var book = await unitOfWork.RepositoryBase<Book>().FindByIdAsync(id);
            if (book is null)
                return NotFound();
            var viewModel = (from b in unitOfWork.BookShopContext.Books.Include(l => l.Language).Include(p => p.Publisher).AsEnumerable()
                             where b.BookId == id
                             select new BooksCreateEditViewModel
                             {
                                 BookId = id.Value,
                                 Title = b.Title,
                                 ISBN = b.ISBN,
                                 IsPublish = b.IsPublished.Value,
                                 NumOfPages = b.NumOfPage,
                                 Price = b.Price,
                                 Stock = b.Stock,
                                 Summary = b.Summary,
                                 Weight = b.Weight,
                                 PublishYear = b.PublishYear,
                                 LanguageID = b.LanguageId,
                                 PublisherID = b.PublisherId,
                                 RecentIsPublish = b.IsPublished.Value,
                                 PublishDate = b.PublishedTime
                             }).First();

            var categories = (from c in unitOfWork.BookShopContext.Book_Categories
                              where c.BookId == id
                              select c.CategoryId).AsEnumerable().ToArray();

            var translators = (from t in unitOfWork.BookShopContext.Book_Translators
                               where t.BookId == id
                               select t.TranslatorId).AsEnumerable().ToArray();

            var authors = (from a in unitOfWork.BookShopContext.Author_Books
                           where a.BookId == id
                           select a.AuthorId).AsEnumerable().ToArray();

            viewModel.CategoryID = categories;
            viewModel.TranslatorID = translators;
            viewModel.AuthorID = authors;
            viewModel.BookSubCategoriesViewModel = new BookSubCategoriesViewModel(unitOfWork.BooksRepository.GetAllCategories(), categories);

            var languages = await unitOfWork.RepositoryBase<Language>().GetAllAsync();
            var publishers = await unitOfWork.RepositoryBase<Publisher>().GetAllAsync();
            var authorsV = await unitOfWork.RepositoryBase<Author>().GetAllAsync();
            var translatorsV = await unitOfWork.RepositoryBase<Translator>().GetAllAsync();

            ViewBag.LanguageID = new SelectList(languages, "LanguageId", "LanguageName");
            ViewBag.PublisherId = new SelectList(publishers, "PublisherId", "PublisherName");
            ViewBag.AuthorID = new SelectList(authorsV.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(translatorsV.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "TranslatorID", "NameFamily");

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(BooksCreateEditViewModel viewModel)
        {
            var languages = await unitOfWork.RepositoryBase<Language>().GetAllAsync();
            var publishers = await unitOfWork.RepositoryBase<Publisher>().GetAllAsync();
            var authorsV = await unitOfWork.RepositoryBase<Author>().GetAllAsync();
            var translatorsV = await unitOfWork.RepositoryBase<Translator>().GetAllAsync();

            viewModel.BookSubCategoriesViewModel = new BookSubCategoriesViewModel(unitOfWork.BooksRepository.GetAllCategories(), viewModel.CategoryID);
            ViewBag.LanguageID = new SelectList(languages, "LanguageId", "LanguageName");
            ViewBag.PublisherId = new SelectList(publishers, "PublisherId", "PublisherName");
            ViewBag.AuthorID = new SelectList(authorsV.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(translatorsV.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "TranslatorID", "NameFamily");

            if (await unitOfWork.BooksRepository.EditBookAsync(viewModel))
                ViewBag.MessageSuccess = "تغییرات با موفقیت ذخیره شد";
            else
                ViewBag.MessageFail = "خطایی رخ داد، مجددا تلاش کنید";

            return View(viewModel);
        }

        public async Task<IActionResult> Download(int id)
        {
            var book = await unitOfWork.RepositoryBase<Book>().FindByIdAsync(id);
            if (book is null)
                return NotFound();
            var path = $"{_webHostEnvironment.WebRootPath}/BooksFile/{book.File}";
            var memory = new MemoryStream() { Position = 0 };
            using FileStream stream = new FileStream(path, FileMode.Open);
            await stream.CopyToAsync(memory);

            return File(memory, GetContentType(path), book.File);
        }

        public string GetContentType(string path)
        {
            var meimeTypes = new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".zip","application/zip" },
                {".rar","application/x-rar" }
            };

            var extension = Path.GetExtension(path).ToLowerInvariant();
            return meimeTypes[extension];
        }

        public async Task<IActionResult> SearchByISBN(string ISBN)
        {
            if (!string.IsNullOrEmpty(ISBN))
            {
                var book = await (from b in unitOfWork.BookShopContext.Books
                                  where b.ISBN == ISBN
                                  select new BooksIndexViewModel
                                  {
                                      Title = b.Title,
                                      Authors = BookShopContext.GetAllAuthors(b.BookId),
                                      Translators = BookShopContext.GetAllTranslators(b.BookId),
                                      Categories = BookShopContext.GetAllCategories(b.BookId)
                                  }).FirstAsync();
                if (book is null)
                {
                    ViewBag.Message = "کتابی با این شابک پیدا نشد";
                    return View();
                }
                return View(book);
            }
            return View();
        }
    }
}