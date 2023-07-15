using BookShop.Models;
using BookShop.Models.Repository;
using BookShop.Models.ViewModels;
using EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BooksController : Controller
    {
        private readonly BookShopContext _context;
        private readonly BooksRepository _repository;
        public BooksController(BookShopContext context, BooksRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Index(string Message, int pageIndex, int row = 5, string sortExpression = "Title", string title = "")
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

            var paging = PagingList.Create(_repository.GetAllBooks(title, "", "", "", "", "", ""), row, pageIndex, sortExpression, "Title");

            paging.RouteValue = new RouteValueDictionary()
            {
                {"row",row },
                {"title",title }
            };

            ViewBag.Language = new SelectList(_context.Languages, "LanguageName", "LanguageName");
            ViewBag.Publisher = new SelectList(_context.Publishers, "PublisherName", "PublisherName");
            ViewBag.Author = new SelectList(_context.Authors.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "NameFamily", "NameFamily");
            ViewBag.Translator = new SelectList(_context.Translators.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "NameFamily", "NameFamily");
            ViewBag.Categories = _repository.GetAllCategories();
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

            return View(_repository.GetAllBooks(booksAdvancedSearch.Title, booksAdvancedSearch.ISBN, booksAdvancedSearch.Language, booksAdvancedSearch.Author, booksAdvancedSearch.Translator, booksAdvancedSearch.Category, booksAdvancedSearch.Publisher));
        }

        public IActionResult Details(int id)
        {
            //var bookInfo = _context.ReadAllBooks.Where(b => b.BookId == id).First();
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            book.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            ViewBag.LanguageID = new SelectList(_context.Languages, "LanguageId", "LanguageName");
            ViewBag.PublisherId = new SelectList(_context.Publishers, "PublisherId", "PublisherName");
            ViewBag.AuthorID = new SelectList(_context.Authors.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(_context.Translators.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "TranslatorID", "NameFamily");
            BooksCreateEditViewModel viewModel = new BooksCreateEditViewModel() { BookSubCategoriesViewModel = new BookSubCategoriesViewModel(_repository.GetAllCategories(), null) };
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BooksCreateEditViewModel viewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(viewModel);
            //}
            List<Book_Translator> translators = new List<Book_Translator>();
            List<Book_Category> categories = new List<Book_Category>();

            if (viewModel.TranslatorID is not null)
                translators = viewModel.TranslatorID.Select(translator => new Book_Translator { TranslatorId = translator }).ToList();
            if (viewModel.CategoryID is not null)
                categories = viewModel.CategoryID.Select(category => new Book_Category { CategoryId = category }).ToList();
            try
            {
                var transaction = await _context.Database.BeginTransactionAsync(); ;
                Book book = new Book
                {
                    Title = viewModel.Title,
                    ISBN = viewModel.ISBN,
                    Summary = viewModel.Summary,
                    NumOfPage = viewModel.NumOfPages,
                    Stock = viewModel.Stock,
                    Price = viewModel.Price,
                    LanguageId = viewModel.LanguageID,
                    IsPublished = viewModel.IsPublish,
                    Weight = viewModel.Weight,
                    PublishYear = viewModel.PublishYear,
                    PublisherId = viewModel.PublisherID,
                    Author_Books = viewModel.AuthorID.Select(author => new Author_Book { AuthorId = author }).ToList(),
                    Book_Translators = translators,
                    Book_Categories = categories
                };

                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index", new { Message = "Failde" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();
            var book = await _context.Books.FindAsync(id);
            if (book is null)
                return NotFound();
            var viewModel = (from b in _context.Books.Include(l => l.Language).Include(p => p.Publisher).AsEnumerable()
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

            var categories = (from c in _context.Book_Categories
                              where c.BookId == id
                              select c.CategoryId).AsEnumerable().ToArray();

            var translators = (from t in _context.Book_Translators
                               where t.BookId == id
                               select t.TranslatorId).AsEnumerable().ToArray();

            var authors = (from a in _context.Author_Books
                           where a.BookId == id
                           select a.AuthorId).AsEnumerable().ToArray();

            viewModel.CategoryID = categories;
            viewModel.TranslatorID = translators;
            viewModel.AuthorID = authors;
            viewModel.BookSubCategoriesViewModel = new BookSubCategoriesViewModel(_repository.GetAllCategories(), categories);
            ViewBag.LanguageID = new SelectList(_context.Languages, "LanguageId", "LanguageName");
            ViewBag.PublisherId = new SelectList(_context.Publishers, "PublisherId", "PublisherName");
            ViewBag.AuthorID = new SelectList(_context.Authors.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(_context.Translators.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "TranslatorID", "NameFamily");

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(BooksCreateEditViewModel viewModel)
        {
            viewModel.BookSubCategoriesViewModel = new BookSubCategoriesViewModel(_repository.GetAllCategories(), viewModel.CategoryID);
            ViewBag.LanguageID = new SelectList(_context.Languages, "LanguageId", "LanguageName");
            ViewBag.PublisherId = new SelectList(_context.Publishers, "PublisherId", "PublisherName");
            ViewBag.AuthorID = new SelectList(_context.Authors.Select(t => new AuthorList { AuthorID = t.AuthorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(_context.Translators.Select(t => new TranslatorList { TranslatorID = t.TranslatorId, NameFamily = $"{t.FirstName} {t.LastName}" }), "TranslatorID", "NameFamily");

            DateTime? dateTime;
            if (!viewModel.RecentIsPublish && viewModel.IsPublish)
                dateTime = DateTime.Now;
            else if (viewModel.RecentIsPublish && !viewModel.IsPublish)
                dateTime = null;
            else
                dateTime = viewModel.PublishDate;
            try
            {
                var book = new Book
                {
                    BookId = viewModel.BookId,
                    Title = viewModel.Title,
                    ISBN = viewModel.ISBN,
                    IsPublished = viewModel.IsPublish,
                    NumOfPage = viewModel.NumOfPages,
                    Price = viewModel.Price,
                    Stock = viewModel.Stock,
                    Summary = viewModel.Summary,
                    Weight = viewModel.Weight,
                    PublishYear = viewModel.PublishYear,
                    LanguageId = viewModel.LanguageID,
                    PublisherId = viewModel.PublisherID,
                    PublishedTime = dateTime
                };

                var categories = (from c in _context.Book_Categories
                                  where c.BookId == viewModel.BookId
                                  select c.CategoryId).AsEnumerable().ToArray();

                var translators = (from t in _context.Book_Translators
                                   where t.BookId == viewModel.BookId
                                   select t.TranslatorId).AsEnumerable().ToArray();

                var authors = (from a in _context.Author_Books
                               where a.BookId == viewModel.BookId
                               select a.AuthorId).AsEnumerable().ToArray();

                var deletedCategories = categories.Except(viewModel.CategoryID);
                var deletedTranslators = translators.Except(viewModel.TranslatorID);
                var deletedAuthors = authors.Except(viewModel.AuthorID);

                var addedCategories = viewModel.CategoryID.Except(categories);
                var addedTranslators = viewModel.TranslatorID.Except(translators);
                var addedAuthors = viewModel.AuthorID.Except(authors);

                if (deletedCategories.Count() > 0)
                    _context.Book_Categories.RemoveRange(deletedCategories.Select(c => new Book_Category { BookId = viewModel.BookId, CategoryId = c }));

                if (deletedTranslators.Count() > 0)
                    _context.Book_Translators.RemoveRange(deletedTranslators.Select(t => new Book_Translator { BookId = viewModel.BookId, TranslatorId = t }));

                if (deletedAuthors.Count() > 0)
                    _context.Author_Books.RemoveRange(deletedAuthors.Select(a => new Author_Book { BookId = viewModel.BookId, AuthorId = a }));

                if (addedCategories.Count() > 0)
                    await _context.Book_Categories.AddRangeAsync(addedCategories.Select(c => new Book_Category { BookId = viewModel.BookId, CategoryId = c }));

                if (addedTranslators.Count() > 0)
                    await _context.Book_Translators.AddRangeAsync(addedTranslators.Select(t => new Book_Translator { BookId = viewModel.BookId, TranslatorId = t }));

                if (addedAuthors.Count() > 0)
                    await _context.Author_Books.AddRangeAsync(addedAuthors.Select(a => new Author_Book { BookId = viewModel.BookId, AuthorId = a }));

                _context.Update(book);
                await _context.SaveChangesAsync();
                ViewBag.MessageSuccess = "تغییرات با موفقیت ذخیره شد";
                return View(viewModel);
            }
            catch
            {
                ViewBag.MessageFail = "خطایی رخ داد، مجددا تلاش کنید";
                return View(viewModel);
            }
        }

        public async Task<IActionResult> SearchByISBN(string ISBN)
        {
            if (!string.IsNullOrEmpty(ISBN))
            {
                var book = await (from b in _context.Books
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
